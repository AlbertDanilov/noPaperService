using noPaperAPI_common.Helpers;
using noPaperService_common.Const;
using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace noPaperService_sender_apt
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();


        static void Main(string[] args)
        {
            /*
             1. Получаем список pv_id, thumbprint для работы 
             2. Находим json файл по pv_id
             3. Отправляем на подпись
             */

            //скрыть консоль
            var handle = GetConsoleWindow();
            ShowWindow(handle, ViewConst.SW_Min);

            LogHelper.RemoveOldLog();

            String jsonPath = "C:\\Rsklad.Documents\\JSON";

            string t = $"Robot run (send json to sign server with apt ecp)";
            Console.WriteLine(t);
            LogHelper.WriteLog(t);
            Console.WriteLine("");

            string routingKeyJsonApt = "json_for_apt";
            var counter = 1;

            //получаем список pv_id, thumbprint для работы
            List<EcpSignData_aptSign> sendItems = DataHelper.GetEcpAptSignData();

            if (sendItems != null && sendItems.Count > 0)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "192.168.0.25",
                    UserName = "artisUser",
                    Password = "250595",
                    VirtualHost = "/",
                    Port = 5672
                };

                try
                {
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "signAptData", type: ExchangeType.Direct, autoDelete: false);

                        //отправляем всё в очередь
                        foreach (EcpSignData_aptSign item in sendItems)
                        {
                            //ищем json файл
                            if (File.Exists(jsonPath + $"\\{item.pv_id}.json"))
                            {
                                Byte[] jsonFile = File.ReadAllBytes(jsonPath + $"\\{item.pv_id}.json");

                                //формируем объект
                                EcpSignData_aptSignData sendData = new EcpSignData_aptSignData()
                                {
                                    pv_id = item.pv_id,
                                    thumbprint = item.thumbprint,
                                    json = jsonFile
                                };

                                //отправляем
                                var body = FormatHelper.ToByteArray(sendData);

                                channel.BasicPublish(exchange: "signAptData",
                                                 routingKey: routingKeyJsonApt,
                                                 basicProperties: null,
                                                 body: body);

                                Console.WriteLine($"Document [{item.pv_id}] send to [{routingKeyJsonApt}] N{counter++}");
                                LogHelper.WriteLog($"Document [{item.pv_id}] send to [{routingKeyJsonApt}] N{counter++}");
                            }                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog($"Exception: {ex.Message}");
                }
                finally
                {
                    LogHelper.WriteLog($"Sended items count: {counter}");
                }
            }
            LogHelper.WriteLog("Robot1 work end.");
            LogHelper.WriteLog("");
        }
    }
}
