using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    public class EcpSignData_pvs
    {
        public Int64 pvs_id { get; set; }
        public Int64? pvs_pv_id { get; set; }
        public Int64? pvs_tov_zap_id { get; set; }
        public Int64? pvs_ttns_id { get; set; }
        public Decimal? pvs_kol_tov { get; set; }
        public EcpSignData_ttns ttnsInfo { get; set; }
    }
}
