using Newtonsoft.Json;
using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using noPaperService_ecpWorker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace noPaperAPI_robot2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
           1. Получаем JSON (byte)
           2. Подписываем byte (генерируется .p7s)
           3. Отправляем в хранилище на 0.25
           */

            Console.WriteLine($"Robot2 run (sign json documents and send to store)");
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

            string routingKeyJson = "json";
            string routingKeyP7s = "p7s";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "signData", type: ExchangeType.Direct, autoDelete: true);

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
                        ReturnData p7s = ECP.Sign("9ddc7831adb7be917f4a7e2d98640cd8d64afe3c", body.ToArray());
                        Console.WriteLine($"Signed document [{doc.pv_id}]");

                        EcpSignData_p7s p7sData = new EcpSignData_p7s() { pv_id = doc.pv_id, sign = (Byte[])p7s.data };
                        Byte[] sendData = FormatHelper.ToByteArray(p7sData);

                        //отправить
                        channel.BasicPublish(exchange: "signData",
                                                     routingKey: routingKeyP7s,
                                                     basicProperties: null,
                                                     body: sendData);
                        Console.WriteLine($"Sended sign [{doc.pv_id}]");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //добавить логгер
                    }
                };

                channel.BasicConsume(queue: queueNameJson,
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine($"Subscribed to the queue JSON '{queueNameJson}'");

                Console.ReadLine();

            }
        }
    }
}
