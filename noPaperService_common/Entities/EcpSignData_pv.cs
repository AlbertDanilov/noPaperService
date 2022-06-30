using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Entities
{
    public class EcpSignData_pv
    {
        public String user_thumbprint { get; set; }
        public Int64 pv_id { get; set; }
        public Int32? pv_nom { get; set; }
        public String pv_num { get; set; }
        public DateTime? pv_create_date { get; set; }
        public DateTime? pv_date { get; set; }
        public DateTime? pv_otr_date { get; set; }
        public Int32? pv_agent_id { get; set; }
        public String pv_agent_agnabbr { get; set; }
        public String pv_agent_printname { get; set; }
        public Int32? pv_agent_anom { get; set; }
        public Int32? pv_sklad_id { get; set; }
        public String pv_sklad_name { get; set; }
        public DateTime? pv_otg_date { get; set; }
        public Int32? pv_plat_id { get; set; }
        public String pv_plat_agnabbr { get; set; }
        public Int16? pv_opl_type_id { get; set; }
        public String pv_opl_type { get; set; }
        public Int32? pv_otr_user_id { get; set; }
        public Int32? pv_zayav_id { get; set; }
        public Int32? pv_dlo_zayav_id { get; set; }
        public DateTime? pv_doing_date { get; set; }
        public Byte? pv_is_mark { get; set; }
        public Int32? pv_work_program_id { get; set; }
        public String pv_work_program_name { get; set; }
        public String pv_otv_fio { get; set; }
        public String pv_sklad_mol { get; set; }
        public String pv_zay_zname { get; set; }
        public DateTime? pv_zay_cdate { get; set; }
        public String pv_reason { get; set; }
        public DateTime? pv_dogovor_date { get; set; }        
        public String pv_zay_lpu { get; set; }        
        public String pv_sklad_iname { get; set; }        
        public List<EcpSignData_pvs> pvsList { get; set; }
    }
}
