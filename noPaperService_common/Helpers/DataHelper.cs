﻿using noPaperService_common.Entities;
using noPaperService_common.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperAPI_common.Helpers
{
    public class DataHelper
    {
        private static string ConnectionSting = "Data Source=192.168.0.35;Initial Catalog=rsklad;User ID=sa;Password=r12sql141007";

        public static List<EcpSignData_pv> GetEcpSignData()
        {
            LogHelper.WriteLog("GetEcpSignData");

            DataTable dt = new DataTable("T");
            List<EcpSignData_pv> docItems = null;

            if (SQLHelper.GetData(ConnectionSting, "DOCS_ECP_SIGN_DATA_GET_ALL", ref dt, null) == false)
            {
                return new List<EcpSignData_pv>();
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
                               pv_apt_accepted_date = d.Field<DateTime?>("pv_apt_accepted_date"),
                               pv_agent_id = d.Field<Int32?>("pv_agent_id"),
                               pv_agent_agnabbr = d.Field<String>("pv_agent_agnabbr"),
                               pv_agent_printname = d.Field<String>("pv_agent_printname"),
                               pv_agent_anom = d.Field<Int32?>("pv_agent_anom"),
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
                               pv_is_mark = d.Field<Byte?>("pv_is_mark"),
                               pv_work_program_id = d.Field<Int16?>("pv_work_program_id"),
                               pv_work_program_name = d.Field<String>("pv_work_program_name"),
                               pv_otv_fio = d.Field<String>("pv_otv_fio"),
                               pv_user_position = d.Field<String>("pv_user_position"),
                               pv_sklad_mol = d.Field<String>("pv_sklad_mol"),
                               pv_zay_zname = d.Field<String>("pv_zay_zname"),
                               pv_zay_cdate = d.Field<DateTime?>("pv_zay_cdate"),
                               pv_reason = d.Field<String>("pv_reason"),
                               pv_dogovor_date = d.Field<DateTime?>("pv_dogovor_date"),
                               pv_zay_lpu = d.Field<String>("pv_zay_lpu"),
                               pv_sklad_iname = d.Field<String>("pv_sklad_iname"),
                               user_thumbprint = d.Field<String>("user_thumbprint")
                           }).Select(ds => new EcpSignData_pv
                           {
                               pv_id = ds.Key.pv_id,
                               pv_nom = ds.Key.pv_nom,
                               pv_num = ds.Key.pv_num,
                               pv_create_date = ds.Key.pv_create_date,
                               pv_date = ds.Key.pv_date,
                               pv_otr_date = ds.Key.pv_otr_date,
                               pv_apt_accepted_date = ds.Key.pv_apt_accepted_date,
                               pv_agent_id = ds.Key.pv_agent_id,
                               pv_agent_agnabbr = ds.Key.pv_agent_agnabbr,
                               pv_agent_printname = ds.Key.pv_agent_printname,
                               pv_agent_anom = ds.Key.pv_agent_anom,
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
                               pv_work_program_id = ds.Key.pv_work_program_id,
                               pv_work_program_name = ds.Key.pv_work_program_name,
                               pv_otv_fio = ds.Key.pv_otv_fio,
                               pv_user_position = ds.Key.pv_user_position,
                               pv_sklad_mol = ds.Key.pv_sklad_mol,
                               pv_zay_zname = ds.Key.pv_zay_zname,
                               pv_zay_cdate = ds.Key.pv_zay_cdate,
                               pv_reason = ds.Key.pv_reason,
                               pv_dogovor_date = ds.Key.pv_dogovor_date,
                               pv_zay_lpu = ds.Key.pv_zay_lpu,
                               pv_sklad_iname = ds.Key.pv_sklad_iname,
                               user_thumbprint = ds.Key.user_thumbprint,
                               pvsList = ds.GroupBy(dss => new
                               {
                                   pvs_id = dss.Field<Int64>("pvs_id"),
                                   pvs_pv_id = dss.Field<Int64>("pvs_pv_id"),
                                   pvs_tov_zap_id = dss.Field<Int64?>("pvs_tov_zap_id"),
                                   pvs_ttns_id = dss.Field<Int64?>("pvs_ttns_id"),
                                   pvs_kol_tov = dss.Field<Decimal?>("pvs_kol_tov"),
                                   pvs_psum_bnds = dss.Field<Decimal?>("pvs_psum_bnds"),
                                   pvs_rsum_nds = dss.Field<Decimal?>("pvs_rsum_nds"),
                                   pvs_psum_nds = dss.Field<Decimal?>("pvs_psum_nds"),
                                   pvs_pcena_bnds = dss.Field<Decimal?>("pvs_pcena_bnds"),
                                   pvs_pcena_nds = dss.Field<Decimal?>("pvs_pcena_nds"),
                                   pvs_ocena_nds = dss.Field<Decimal?>("pvs_ocena_nds"),
                                   pvs_osum_nds = dss.Field<Decimal?>("pvs_osum_nds"),
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
                                   ttns_temp_regim_id = dss.Field<Int16?>("ttns_temp_regim_id"),
                                   ttns_sert_num = dss.Field<String>("ttns_sert_num"),
                                   ttns_sert_date_po = dss.Field<DateTime?>("ttns_sert_date_po"),
                                   ttns_sert_date_s = dss.Field<DateTime?>("ttns_sert_date_s"),
                                   ttns_ed_shortname = dss.Field<String>("ttns_ed_shortname"),
                                   ttns_temp_regim_name = dss.Field<String>("ttns_temp_regim_name"),
                                   pvs_dg_num = dss.Field<String>("pvs_dg_num"),
                                   docs_p_jnvls = dss.Field<Int32?>("docs_p_jnvls"),
                                   docs_p_mnn = dss.Field<String>("docs_p_mnn"),
                                   docs_p_tn = dss.Field<String>("docs_p_tn"),
                                   docs_p_fv_doz = dss.Field<String>("docs_p_fv_doz"),
                                   docs_p_proizv = dss.Field<String>("docs_p_proizv"),
                                   docs_p_prcena_proizv = dss.Field<Decimal?>("docs_p_prcena_proizv"),
                                   docs_prcena_bnds = dss.Field<Decimal?>("docs_prcena_bnds"),
                                   docs_dt_prcena_bnds = dss.Field<DateTime?>("docs_dt_prcena_bnds"),
                                   docs_prcena_nds = dss.Field<Decimal?>("docs_prcena_nds"),
                                   docs_ocena_bnds = dss.Field<Decimal?>("docs_ocena_bnds"),
                                   nac_sum_val = dss.Field<Decimal?>("nac_sum_val"),
                                   nac_prc_val = dss.Field<Decimal?>("nac_prc_val"),
                                   nac_sum_val_p = dss.Field<Decimal?>("nac_sum_val_p"),
                                   nac_prc_val_p = dss.Field<Decimal?>("nac_prc_val_p"),
                                   nac_sum_val_p2 = dss.Field<Decimal?>("nac_sum_val_p2"),
                                   nac_prc_val_p2 = dss.Field<Decimal?>("nac_prc_val_p2"),
                                   rcena_bnds = dss.Field<Decimal?>("rcena_bnds"),
                                   nac_sum_rozn_val = dss.Field<Decimal?>("nac_sum_rozn_val"),
                                   nac_prc_rozn_val = dss.Field<Decimal?>("nac_prc_rozn_val")
                               }).Select(dss => new EcpSignData_pvs
                               {
                                   pvs_id = dss.Key.pvs_id,
                                   pvs_tov_zap_id = dss.Key.pvs_tov_zap_id,
                                   pvs_ttns_id = dss.Key.pvs_ttns_id,
                                   pvs_kol_tov = dss.Key.pvs_kol_tov,
                                   pvs_psum_bnds = dss.Key.pvs_psum_bnds,
                                   pvs_rsum_nds = dss.Key.pvs_rsum_nds,
                                   pvs_psum_nds = dss.Key.pvs_psum_nds,
                                   pvs_pcena_bnds = dss.Key.pvs_pcena_bnds,
                                   pvs_pcena_nds = dss.Key.pvs_pcena_nds,
                                   pvs_ocena_nds = dss.Key.pvs_ocena_nds,
                                   pvs_osum_nds = dss.Key.pvs_osum_nds,
                                   pvs_dg_num = dss.Key.pvs_dg_num,
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
                                       ttns_temp_regim_id = dss.Key.ttns_temp_regim_id,
                                       ttns_sert_num = dss.Key.ttns_sert_num,
                                       ttns_sert_date_po = dss.Key.ttns_sert_date_po,
                                       ttns_sert_date_s = dss.Key.ttns_sert_date_s,
                                       ttns_ed_shortname = dss.Key.ttns_ed_shortname,
                                       ttns_temp_regim_name = dss.Key.ttns_temp_regim_name,
                                       docs_p_jnvls = dss.Key.docs_p_jnvls,
                                       docs_p_mnn = dss.Key.docs_p_mnn,
                                       docs_p_tn = dss.Key.docs_p_tn,
                                       docs_p_fv_doz = dss.Key.docs_p_fv_doz,
                                       docs_p_proizv = dss.Key.docs_p_proizv,
                                       docs_p_prcena_proizv = dss.Key.docs_p_prcena_proizv,
                                       docs_prcena_bnds = dss.Key.docs_prcena_bnds,
                                       docs_dt_prcena_bnds = dss.Key.docs_dt_prcena_bnds,
                                       docs_prcena_nds = dss.Key.docs_prcena_nds,
                                       docs_ocena_bnds = dss.Key.docs_ocena_bnds,
                                       nac_sum_val = dss.Key.nac_sum_val,
                                       nac_prc_val = dss.Key.nac_prc_val,
                                       nac_sum_val_p = dss.Key.nac_sum_val_p,
                                       nac_prc_val_p = dss.Key.nac_prc_val_p,
                                       nac_sum_val_p2 = dss.Key.nac_sum_val_p2,
                                       nac_prc_val_p2 = dss.Key.nac_prc_val_p2,
                                       rcena_bnds = dss.Key.rcena_bnds,
                                       nac_sum_rozn_val = dss.Key.nac_sum_rozn_val,
                                       nac_prc_rozn_val = dss.Key.nac_prc_rozn_val
                                   }
                               }).ToList()
                           }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"GroupBy Exception: {ex.Message}");
                return new List<EcpSignData_pv>();
            }

            return docItems;
        }

        public static List<EcpSignData_aptSign> GetEcpAptSignData()
        {
            LogHelper.WriteLog("GetEcpAptSignData");

            DataTable dt = new DataTable("T");
            List<EcpSignData_aptSign> signItems = null;

            if (SQLHelper.GetData(ConnectionSting, "DOCS_ECP_APT_SIGN_DATA_GET_ALL", ref dt, null) == false) 
            { 
                return new List<EcpSignData_aptSign>(); 
            }

            try
            {
                signItems = dt.AsEnumerable()
                            .GroupBy(x => new { 
                                                   pv_id = x.Field<Int64>("pv_id"),
                                                   thumbprint = x.Field<String>("thumbprint"),
                                                   apt_accepted_thumbprint = x.Field<String>("apt_accepted_thumbprint"),
                                                   apt_accepted_kassir_id = x.Field<int>("apt_accepted_kassir_id"),
                                                   FIO = x.Field<String>("FIO")
                            }).Select(y => new EcpSignData_aptSign
                            { 
                                pv_id = y.Key.pv_id,
                                thumbprint = y.Key.thumbprint,
                                apt_accepted_thumbprint = y.Key.apt_accepted_thumbprint,
                                apt_accepted_kassir_id = y.Key.apt_accepted_kassir_id,
                                FIO = y.Key.FIO
                            }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"GroupBy Exception: {ex.Message}");
                return new List<EcpSignData_aptSign>();
            }

            return signItems;
        }

        public static void sendedSet(List<long> sendedIds) 
        {
            LogHelper.WriteLog("sendedSet");

            DataTable dt = new DataTable("T");
            dt.Columns.Add("pv_id", typeof(long));

            foreach(long id in sendedIds)
            {
                dt.Rows.Add(id);
            }

            Dictionary<string, Object> param = new Dictionary<string, object>();

            param.Add("dt", dt);

            LogHelper.WriteLog($"sendedSet, count = {sendedIds.Count}");

            SQLHelper.Execute(ConnectionSting, "DOCS_ECP_SENDED_SET", param);
        }

        public static void signedSet(long signedId)
        {
            LogHelper.WriteLog($"signedSet = {signedId}");

            Dictionary<string, Object> param = new Dictionary<string, object>();

            param.Add("pv_id", signedId);

            SQLHelper.Execute(ConnectionSting, "DOCS_ECP_SIGNED_SET", param);
        }

        public static void signedAptSet(long signedId)
        {
            LogHelper.WriteLog($"signedAptSet = {signedId}");

            Dictionary<string, Object> param = new Dictionary<string, object>();

            param.Add("pv_id", signedId);

            SQLHelper.Execute(ConnectionSting, "DOCS_ECP_APT_SIGNED_SET", param);          
        }
    }
}

