using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    [Serializable()]
    public class EcpSignData_aptSign
    {
        public Int64 pv_id { get; set; }
        public String thumbprint { get; set; }
    }

    [Serializable()]
    public class EcpSignData_aptSignData
    {
        public Int64 pv_id { get; set; }
        public String thumbprint { get; set; }
        public Byte[] json { get; set; }
    }
}
