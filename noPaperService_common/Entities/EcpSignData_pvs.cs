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
        public Decimal? pvs_psum_bnds { get; set; }
        public Decimal? pvs_rsum_nds { get; set; }
        public Decimal? pvs_psum_nds { get; set; }
        public Decimal? pvs_pcena_bnds { get; set; }
        public Decimal? pvs_pcena_nds { get; set; }
        public Decimal? pvs_ocena_nds { get; set; }
        public Decimal? pvs_osum_nds { get; set; }
        public String pvs_dg_num { get; set; }
        public EcpSignData_ttns ttnsInfo { get; set; }
    }
}
