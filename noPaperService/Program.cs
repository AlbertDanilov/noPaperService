using noPaperAPI_robot1.DAL.Helpers;
using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace noPaperAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            /*             
            1. Берем список pv_id для работы                         
                    Накладные с типом "Расход в аптеку" со статусом "Отработан"
                        select pvo.pv_id 
                        from pri_voz_worked_out pvo with(nolock)
                        group by pvo.pv_id,
		                         pvo.signed
                        having pvo.signed = 0
            2. Генерируем JSON с информацией о накладной
            3. Отправляем JSON
                    Для подписи ЭЦП на 0.239 сервер
                    В хранилище на 0.25 сервер
            4. Ставим отметку sended в таблице pri_voz_worked_out            
            */

            LogHelper.RemoveOldLog();

            string t = $"Robot1 run (send documents to store and for sign)";
            Console.WriteLine(t);
            LogHelper.WriteLog(t);
            Console.WriteLine("");
            
            string routingKeyJson = "json";
            var counter = 1;

            //получаем данные о накладных для подписывания
            List<EcpSignData_pv> docItems = DataHelper.GetEcpSignData();

            if (docItems != null && docItems.Count > 0) {           

                var factory = new ConnectionFactory()
                {
                    HostName = "192.168.0.25",
                    UserName = "artisUser",
                    Password = "250595",
                    VirtualHost = "/",
                    Port = 5672
                };

                try {
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "signData", type: ExchangeType.Direct, autoDelete: true);

                        //отправляем всё в очередь
                        foreach (EcpSignData_pv item in docItems)
                        {
                            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(item));

                            channel.BasicPublish(exchange: "signData",
                                                 routingKey: routingKeyJson,
                                                 basicProperties: null,
                                                 body: body);

                            Console.WriteLine($"Document [{item.pv_id}] send to [{routingKeyJson}] N{counter++}");
                        }
                    }

                    LogHelper.WriteLog($"Sended document count: {counter}");
                }
                catch (Exception ex) {
                    LogHelper.WriteLog($"Exception: {ex.Message}");
                }                
            }

            LogHelper.WriteLog("Robot1 work end.");
        }
    }
}
