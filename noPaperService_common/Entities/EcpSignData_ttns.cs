using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    public class EcpSignData_ttns
    {
        public Int64 ttns_id { get; set; }
        public Int32? ttns_shifr_nom { get; set; }
        public String ttns_shifr { get; set; }
        public Int32? ttns_ttn_id { get; set; }
        public Int32? ttns_prep_id { get; set; }
        public String ttns_p_name_s { get; set; }
        public Int32? ttns_parus_nommodif_id { get; set; }
        public String ttns_nommodif { get; set; }
        public String ttns_seria { get; set; }
        public DateTime? ttns_sgod { get; set; }
        public Decimal? ttns_nds_val { get; set; }
        public Decimal? ttns_nds_i_val { get; set; }
        public Decimal? ttns_prcena_bnds { get; set; }
        public Decimal? ttns_r_nac { get; set; }
        public Decimal? ttns_r_nac2 { get; set; }
        public Decimal? ttns_opt_nac { get; set; }
        public Decimal? ttns_ocena_nds { get; set; }
        public Decimal? ttns_rcena_nds { get; set; }
        public DateTime? ttns_izg_date { get; set; }
        public Int64? ttns_dogovor_spec_id { get; set; }
        public Int32? ttns_zayav_type_id { get; set; }
        public Byte? ttns_fixed_rcena { get; set; }
        public Int16? ttns_temp_regim_id { get; set; }
        public String ttns_sert_num { get; set; }
        public DateTime? ttns_sert_date_po { get; set; }
        public String ttns_ed_shortname { get; set; }
        public String ttns_temp_regim_name { get; set; }
        public DateTime? ttns_sert_date_s { get; set; }

        public Int32? docs_p_jnvls { get; set; }
        public String docs_p_mnn { get; set; }
        public String docs_p_tn { get; set; }
        public String docs_p_proizv { get; set; }
        public Decimal? docs_p_prcena_proizv { get; set; }
        public Decimal? docs_prcena_bnds { get; set; }
        public Decimal? docs_prcena_nds { get; set; }
        public Decimal? docs_ocena_bnds { get; set; }
        public Decimal? nac_sum_val { get; set; }
        public Decimal? nac_prc_val { get; set; }
        public Decimal? nac_sum_val_p { get; set; }
        public Decimal? nac_prc_val_p { get; set; }
        public Decimal? nac_sum_val_p2 { get; set; }
        public Decimal? nac_prc_val_p2 { get; set; }
        public Decimal? rcena_bnds { get; set; }
        public Decimal? nac_sum_rozn_val { get; set; }
        public Decimal? nac_prc_rozn_val { get; set; }
    }
}
