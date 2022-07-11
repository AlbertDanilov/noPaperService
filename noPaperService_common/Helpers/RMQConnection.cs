using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Helpers
{
    public class RMQConnection
    {
        private static ConnectionFactory instance;

        public static ConnectionFactory getInstance()
        {
            if (instance == null) instance = new ConnectionFactory()
            {
                HostName = "192.168.0.25",
                UserName = "artisUser",
                Password = "250595",
                VirtualHost = "/",
                Port = 5672
            };

            return instance;
        }
    }
}
