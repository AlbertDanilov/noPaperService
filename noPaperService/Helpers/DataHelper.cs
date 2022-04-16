using Newtonsoft.Json;
using noPaperService_common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperAPI_robot1.DAL.Helpers
{
    class DataHelper
    {
        private static string ConnectionSting = "Data Source=192.168.0.35;Initial Catalog=rsklad;User ID=sa;Password=r12sql141007";

        public static List<EcpSignData_pv> GetEcpSignData()
        {
            DataTable dt = new DataTable("T");
            List<EcpSignData_pv> docItems = null;

            try
            {
                using (var con = new SqlConnection(ConnectionSting))
                {
                    using (var cmd = new SqlCommand("", con))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.SelectCommand.CommandText = "DOCS_ECP_SIGN_DATA_GET_ALL";
                            da.SelectCommand.Parameters.Clear();
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch
            {
                //добавить логгер
            }

            try
            {
                docItems = dt.AsEnumerable()
                           .GroupBy(d => new
                           {
                               pv_id = d.Field<Int64>("pv_id"),
                               pv_nom = d.Field<Int32?>("pv_nom"),
                               pv_num = d.Field<String>("pv_num"),
                               pv_create_date = d.Field<DateTime?>("pv_create_date"),
                               pv_date = d.Field<DateTime?>("pv_date"),
                               pv_otr_date = d.Field<DateTime?>("pv_otr_date"),
                               pv_agent_id = d.Field<Int32?>("pv_agent_id"),
                               pv_agent_agnabbr = d.Field<String>("pv_agent_agnabbr"),
                               pv_sklad_id = d.Field<Int32?>("pv_sklad_id"),
                               pv_sklad_name = d.Field<String>("pv_sklad_name"),
                               pv_otg_date = d.Field<DateTime?>("pv_otg_date"),
                               pv_plat_id = d.Field<Int32?>("pv_plat_id"),
                               pv_plat_agnabbr = d.Field<String>("pv_plat_agnabbr"),
                               pv_opl_type_id = d.Field<Int16?>("pv_opl_type_id"),
                               pv_opl_type = d.Field<String>("pv_opl_type"),
                               pv_otr_user_id = d.Field<Int32?>("pv_otr_user_id"),
                               pv_zayav_id = d.Field<Int32?>("pv_zayav_id"),
                               pv_dlo_zayav_id = d.Field<Int32?>("pv_dlo_zayav_id"),
                               pv_doing_date = d.Field<DateTime?>("pv_doing_date"),
                               pv_is_mark = d.Field<Byte?>("pv_is_mark")
                           }).Select(ds => new EcpSignData_pv
                           {
                               pv_id = ds.Key.pv_id,
                               pv_nom = ds.Key.pv_nom,
                               pv_num = ds.Key.pv_num,
                               pv_create_date = ds.Key.pv_create_date,
                               pv_date = ds.Key.pv_date,
                               pv_otr_date = ds.Key.pv_otr_date,
                               pv_agent_id = ds.Key.pv_agent_id,
                               pv_agent_agnabbr = ds.Key.pv_agent_agnabbr,
                               pv_sklad_id = ds.Key.pv_sklad_id,
                               pv_sklad_name = ds.Key.pv_sklad_name,
                               pv_otg_date = ds.Key.pv_otg_date,
                               pv_plat_id = ds.Key.pv_plat_id,
                               pv_plat_agnabbr = ds.Key.pv_plat_agnabbr,
                               pv_opl_type_id = ds.Key.pv_opl_type_id,
                               pv_opl_type = ds.Key.pv_opl_type,
                               pv_otr_user_id = ds.Key.pv_otr_user_id,
                               pv_zayav_id = ds.Key.pv_zayav_id,
                               pv_dlo_zayav_id = ds.Key.pv_dlo_zayav_id,
                               pv_doing_date = ds.Key.pv_doing_date,
                               pv_is_mark = ds.Key.pv_is_mark,
                               pvsList = ds.GroupBy(dss => new
                               {
                                   pvs_id = dss.Field<Int64>("pvs_id"),
                                   pvs_pv_id = dss.Field<Int64>("pvs_pv_id"),
                                   pvs_tov_zap_id = dss.Field<Int64?>("pvs_tov_zap_id"),
                                   pvs_ttns_id = dss.Field<Int64?>("pvs_ttns_id"),
                                   pvs_kol_tov = dss.Field<Decimal?>("pvs_kol_tov"),
                                   ttns_id = dss.Field<Int64>("ttns_id"),
                                   ttns_shifr_nom = dss.Field<Int32?>("ttns_shifr_nom"),
                                   ttns_shifr = dss.Field<String>("ttns_shifr"),
                                   ttns_ttn_id = dss.Field<Int32?>("ttns_ttn_id"),
                                   ttns_prep_id = dss.Field<Int32?>("ttns_prep_id"),
                                   ttns_p_name_s = dss.Field<String>("ttns_p_name_s"),
                                   ttns_parus_nommodif_id = dss.Field<Int32?>("ttns_parus_nommodif_id"),
                                   ttns_nommodif = dss.Field<String>("ttns_nommodif"),
                                   ttns_seria = dss.Field<String>("ttns_seria"),
                                   ttns_sgod = dss.Field<DateTime?>("ttns_sgod"),
                                   ttns_nds_val = dss.Field<Decimal?>("ttns_nds_val"),
                                   ttns_nds_i_val = dss.Field<Decimal?>("ttns_nds_i_val"),
                                   ttns_prcena_bnds = dss.Field<Decimal?>("ttns_prcena_bnds"),
                                   ttns_r_nac = dss.Field<Decimal?>("ttns_r_nac"),
                                   ttns_r_nac2 = dss.Field<Decimal?>("ttns_r_nac2"),
                                   ttns_opt_nac = dss.Field<Decimal?>("ttns_opt_nac"),
                                   ttns_ocena_nds = dss.Field<Decimal?>("ttns_ocena_nds"),
                                   ttns_rcena_nds = dss.Field<Decimal?>("ttns_rcena_nds"),
                                   ttns_izg_date = dss.Field<DateTime?>("ttns_izg_date"),
                                   ttns_dogovor_spec_id = dss.Field<Int64?>("ttns_dogovor_spec_id"),
                                   ttns_zayav_type_id = dss.Field<Int32?>("ttns_zayav_type_id"),
                                   ttns_fixed_rcena = dss.Field<Byte?>("ttns_fixed_rcena"),
                                   ttns_temp_regim_id = dss.Field<Int16?>("ttns_temp_regim_id")
                               }).Select(dss => new EcpSignData_pvs
                               {
                                   pvs_id = dss.Key.pvs_id,
                                   pvs_tov_zap_id = dss.Key.pvs_tov_zap_id,
                                   pvs_ttns_id = dss.Key.pvs_ttns_id,
                                   pvs_kol_tov = dss.Key.pvs_kol_tov,
                                   ttnsInfo = new EcpSignData_ttns
                                   {
                                       ttns_id = dss.Key.ttns_id,
                                       ttns_shifr_nom = dss.Key.ttns_shifr_nom,
                                       ttns_shifr = dss.Key.ttns_shifr,
                                       ttns_ttn_id = dss.Key.ttns_ttn_id,
                                       ttns_prep_id = dss.Key.ttns_prep_id,
                                       ttns_p_name_s = dss.Key.ttns_p_name_s,
                                       ttns_parus_nommodif_id = dss.Key.ttns_parus_nommodif_id,
                                       ttns_nommodif = dss.Key.ttns_nommodif,
                                       ttns_seria = dss.Key.ttns_seria,
                                       ttns_sgod = dss.Key.ttns_sgod,
                                       ttns_nds_val = dss.Key.ttns_nds_val,
                                       ttns_nds_i_val = dss.Key.ttns_nds_i_val,
                                       ttns_prcena_bnds = dss.Key.ttns_prcena_bnds,
                                       ttns_r_nac = dss.Key.ttns_r_nac,
                                       ttns_r_nac2 = dss.Key.ttns_r_nac2,
                                       ttns_opt_nac = dss.Key.ttns_opt_nac,
                                       ttns_ocena_nds = dss.Key.ttns_ocena_nds,
                                       ttns_rcena_nds = dss.Key.ttns_rcena_nds,
                                       ttns_izg_date = dss.Key.ttns_izg_date,
                                       ttns_dogovor_spec_id = dss.Key.ttns_dogovor_spec_id,
                                       ttns_zayav_type_id = dss.Key.ttns_zayav_type_id,
                                       ttns_fixed_rcena = dss.Key.ttns_fixed_rcena,
                                       ttns_temp_regim_id = dss.Key.ttns_temp_regim_id
                                   }
                               }).ToList()
                           }).ToList();
            }
            catch
            {
                //добавить логгер
            }

            return docItems;
        }
    }
}

