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
        public String apt_accepted_thumbprint { get; set; }
        public String FIO { get; set; }
        public int apt_accepted_kassir_id { get; set; }
    }

    [Serializable()]
    public class EcpSignData_aptSignData
    {
        public Int64 pv_id { get; set; }
        public String thumbprint { get; set; }
        public String apt_accepted_thumbprint { get; set; }
        public String FIO { get; set; }
        public int apt_accepted_kassir_id { get; set; }
        public Byte[] json { get; set; }
    }
}
