using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Helpers
{
    public class FormatHelper
    {
        public static byte[] ToByteArray<T>(T obj)
        {
            try {
                if (obj == null)
                    return null;
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }
            catch (Exception ex) {
                LogHelper.WriteLog($"ToByteArray Exception: {ex.Message}");
                return null;
            }            
        }

        public static T FromByteArray<T>(byte[] data)
        {
            try {
                if (data == null)
                    return default(T);
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    object obj = bf.Deserialize(ms);
                    return (T)obj;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"FromByteArray Exception: {ex.Message}");
                return default(T);
            }            
        }
    }
}
