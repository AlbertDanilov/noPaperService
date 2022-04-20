using noPaperAPI_common.Helpers;
using noPaperService_common.Const;
using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace noPaperService_signedSetter
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
           1. Получаем List<long>
           2. Отмечаем в базе signed по всем зм_шв
           */

            //скрыть консоль
            var handle = GetConsoleWindow();
            ShowWindow(handle, ViewConst.SW_Min);

            LogHelper.RemoveOldLog();

            string t = $"Robot4 run (set signed)";
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

            string routingKeySigned = "signedIds";

            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "signData", type: ExchangeType.Direct, autoDelete: false);

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName,
                                      exchange: "signData",
                                      routingKey: routingKeySigned);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, e) =>
                    {
                        //получаем, конвертируем
                        var body = e.Body;
                        //long sigedId = FormatHelper.FromByteArray<long>(body.ToArray());
                        long sigedId = BitConverter.ToInt64(body.ToArray(), 0);

                        if (sigedId > 0)
                        {
                            //проставляем в базе sigedId
                            DataHelper.signedSet(sigedId);
                        }
                        else {
                            LogHelper.WriteLog($"sigedIds is null or Count = 0");
                        }
                    };

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.ReadLine();
                }                
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"Exception: {ex.Message}");
            }
            finally
            {
                LogHelper.WriteLog("Robot4 work end.");
                LogHelper.WriteLog("");
            }
        }
    }
}
