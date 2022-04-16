using Newtonsoft.Json;
using noPaperService_common.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
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

                var queueNameJson = channel.QueueDeclare().QueueName;
                var queueNameP7s = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueNameJson,
                                  exchange: "signData",
                                  routingKey: routingKeyJson);

                channel.QueueBind(queue: queueNameP7s,
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
                                var message = Encoding.UTF8.GetString(body.ToArray());
                                EcpSignData_pv doc = JsonConvert.DeserializeObject<EcpSignData_pv>(message);
                                Console.WriteLine($"Received document [{doc.pv_id}] N{counterJson++}");
                                File.WriteAllText(jsonPath + $"\\{doc.pv_id}.json", message);
                                break;
                            case "p7s":
                                var message2 = Encoding.UTF8.GetString(body.ToArray());
                                EcpSignData_p7s signData = JsonConvert.DeserializeObject<EcpSignData_p7s>(message2);
                                Console.WriteLine($"Received document [{signData.pv_id}] N{counterP7s++}");
                                File.WriteAllText(p7sPath + $"\\{signData.pv_id}.p7s", message2);

                                //отправить сообщение о подписывании {signData.pv_id}

                                break;
                        }
                    }
                    catch
                    {
                        //добавить логгер
                    }
                };

                channel.BasicConsume(queue: queueNameJson,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine($"Subscribed to the queue JSON '{queueNameJson}'");
                Console.WriteLine($"Subscribed to the queue JSON '{queueNameP7s}'");

                Console.ReadLine();

            }
        }
    }
}
