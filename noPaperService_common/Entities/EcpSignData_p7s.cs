using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    [Serializable()]
    public class EcpSignData_p7s
    {
        public Int64 pv_id { get; set; }
        public Byte[] sign { get; set; }
    }
}
