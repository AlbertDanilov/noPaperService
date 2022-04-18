using Newtonsoft.Json;
using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace noPaperAPI_robot3
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            1. Получаем JSON (byte) или p7s (byte)
            2. Сохраняем в хранилище
            3. Отправляем информацию о подписывании на 0.35 (ставим отметку signed в таблице pri_voz_worked_out)
            */

            String jsonPath = "C:\\Rsklad.Documents\\JSON";
            String p7sPath = "C:\\Rsklad.Documents\\P7S";

            Directory.CreateDirectory("C:\\Rsklad.Documents");
            Directory.CreateDirectory("C:\\Rsklad.Documents\\JSON");
            Directory.CreateDirectory("C:\\Rsklad.Documents\\P7S");

            Console.WriteLine($"Robot3 run (save documents and signs to store)");
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
            var counterP7s = 1;

            string routingKeyJson = "json";
            string routingKeyP7s = "p7s";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "signData", type: ExchangeType.Direct, autoDelete: true);

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                                  exchange: "signData",
                                  routingKey: routingKeyJson);

                channel.QueueBind(queue: queueName,
                                  exchange: "signData",
                                  routingKey: routingKeyP7s);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, e) =>
                {
                    try
                    {
                        var body = e.Body;

                        switch (e.RoutingKey)
                        {
                            case "json":
                                //получаем
                                var message = Encoding.UTF8.GetString(body.ToArray());

                                //конвертируем
                                EcpSignData_pv doc = JsonConvert.DeserializeObject<EcpSignData_pv>(message);
                                Console.WriteLine($"Received document [{doc.pv_id}] N{counterJson++}");

                                //сохраняем
                                File.WriteAllText(jsonPath + $"\\{doc.pv_id}.json", message);
                                break;

                            case "p7s":
                                //получаем, конвертируем
                                EcpSignData_p7s signData = FormatHelper.FromByteArray<EcpSignData_p7s>(body.ToArray());
                                Console.WriteLine($"Received document [{signData.pv_id}] N{counterP7s++}");

                                //сохраняем
                                File.WriteAllBytes(p7sPath + $"\\{signData.pv_id}.p7s", signData.sign);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        //добавить логгер
                    }
                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine($"Subscribed to the queue JSON '{queueName}'");
                Console.WriteLine($"Subscribed to the queue P7S '{queueName}'");

                Console.ReadLine();

            }
        }
    }
}
