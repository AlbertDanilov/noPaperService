using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace noPaperService_common.Helpers
{
    public class LogHelper
    {
        private static string logPath = $"{Environment.GetFolderPath(SpecialFolder.CommonApplicationData)}\\RabbitMq\\RobotLogs";

        private static void init()
        {
            try {
                Directory.CreateDirectory($"{Environment.GetFolderPath(SpecialFolder.CommonApplicationData)}\\RabbitMq");
                Directory.CreateDirectory($"{Environment.GetFolderPath(SpecialFolder.CommonApplicationData)}\\RabbitMq\\RobotLogs");
            }
            catch { }
        }

        public static void RemoveOldLog()
        {
            init();

            try {
                DirectoryInfo dInfo = new DirectoryInfo(logPath);

                foreach (FileInfo fi in dInfo.GetFiles())
                {
                    if ((DateTime.Now - fi.LastWriteTime).TotalDays > 7)
                    {
                        File.Delete(fi.FullName);
                    }
                }
            }
            catch { }            
        }

        public static void WriteLog(string text)
        {
            init();

            try {
                if (text.Length == 0) {
                    File.AppendAllText($"{logPath}\\Log_{DateTime.Now.ToString("yyyy_MM_dd")}.txt", $"\n");
                }
                else {
                    File.AppendAllText($"{logPath}\\Log_{DateTime.Now.ToString("yyyy_MM_dd")}.txt", $"{DateTime.Now} - {text}\n");
                }                
            }
            catch { }
        }
    }
}
