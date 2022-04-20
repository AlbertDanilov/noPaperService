using Newtonsoft.Json;
using noPaperService_common.Const;
using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using noPaperService_ecpWorker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace noPaperAPI_robot2
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
           1. Получаем JSON (byte)
           2. Подписываем byte (генерируется .p7s)
           3. Отправляем в хранилище на 0.25
           */

            //скрыть консоль
            var handle = GetConsoleWindow();
            ShowWindow(handle, ViewConst.SW_Min);

            string t = $"Robot2 run (sign json documents and send to store)";
            Console.WriteLine(t);
            LogHelper.WriteLog(t);
            Console.WriteLine("");

            var factory = new ConnectionFactory()
            {
                HostName = "192.168.0.25",
                UserName = "artisUser",
                Password = "250595",
                VirtualHost = "/",
                Port = 5672
            };

            var counterJson = 1;
            var sendedSign = 1;

            string routingKeyJson = "json";
            string routingKeyP7s = "p7s";

            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "signData", type: ExchangeType.Direct, autoDelete: false);

                    var queueNameJson = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueNameJson,
                                      exchange: "signData",
                                      routingKey: routingKeyJson);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, e) =>
                    {
                        try
                        {
                            var body = e.Body;
                            var message = Encoding.UTF8.GetString(body.ToArray());
                            EcpSignData_pv doc = JsonConvert.DeserializeObject<EcpSignData_pv>(message);
                            Console.WriteLine($"Received document [{doc.pv_id}] N{counterJson++}");

                            //подписать
                            ReturnData p7s = ECP.Sign("9B552D4E124F2190270D3E222D675B9656C9DAFF", body.ToArray());
                            Console.WriteLine($"Signed document [{doc.pv_id}]");

                            EcpSignData_p7s p7sData = new EcpSignData_p7s() { pv_id = doc.pv_id, sign = (Byte[])p7s.data };
                            Byte[] sendData = FormatHelper.ToByteArray(p7sData);

                            if (sendData != null && sendData.Length > 0)
                            {
                                //отправить
                                channel.BasicPublish(exchange: "signData",
                                                             routingKey: routingKeyP7s,
                                                             basicProperties: null,
                                                             body: sendData);
                                Console.WriteLine($"Sended sign [{doc.pv_id}] N{sendedSign++}");
                            }
                            else
                            {
                                Console.WriteLine($"sendData is null or Length = 0");
                                LogHelper.WriteLog($"sendData is null or Length = 0");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.WriteLog($"Received Exception: {ex.Message}");
                        }
                    };

                    LogHelper.WriteLog($"Received json count: {counterJson}");
                    LogHelper.WriteLog($"Sended sign count: {sendedSign}");

                    channel.BasicConsume(queue: queueNameJson,
                                        autoAck: true,
                                        consumer: consumer);

                    Console.WriteLine($"Subscribed to the queue JSON '{queueNameJson}'");

                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"Exception: {ex.Message}");
            }
            finally {
                LogHelper.WriteLog("Robot2 work end.");
                LogHelper.WriteLog("");
            }
        }
    }
}
