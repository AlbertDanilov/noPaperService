using noPaperService_common.Entities;
using noPaperService_ecpWorker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //читаем подпись в массив байтов
            byte[] sign = File.ReadAllBytes("C:\\Users\\user\\Desktop\\RMQ\\1585758.p7s");

            //считываем подписи из файла
            List<SignComponent> signersList = X509.PKCS_7.GetSigners(sign);

            Console.ReadLine();
        }
    }
}
