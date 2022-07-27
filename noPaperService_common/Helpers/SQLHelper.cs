using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noPaperService_common.Helpers
{
    public static class SQLHelper
    {
        public static bool GetData(string connectionString, string spName, ref DataTable dt, Dictionary<string, Object> param = null)
        {
            if (dt == null) return false;

            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand("", con))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.SelectCommand.CommandTimeout = 300000;
                            da.SelectCommand.CommandText = spName;
                            da.SelectCommand.Parameters.Clear();

                            if (param != null)
                            {
                                foreach (KeyValuePair<string, Object> item in param)
                                {
                                    SqlDbType type = SqlDbType.VarChar;

                                    switch (item.Value.GetType().Name)
                                    {
                                        case nameof(System.String):
                                            type = SqlDbType.VarChar;
                                            break;
                                        case nameof(DataTable):
                                            type = SqlDbType.Structured;
                                            break;
                                    }

                                    da.SelectCommand.Parameters.Add(item.Key, type);
                                    da.SelectCommand.Parameters[0].Value = item.Value;
                                }
                            }

                            da.Fill(dt);
                        }
                    }
                }
                LogHelper.WriteLog($"{spName}, rows.count = {dt.Rows.Count}");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"{spName}, Exception: {ex.Message}");
                return false;
            }
        }

        public static bool Execute(string connectionString, string spName, Dictionary<string, Object> param = null)
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    using (var cmd = new SqlCommand("", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = spName;
                        cmd.Parameters.Clear();

                        if (param != null)
                        {
                            foreach (KeyValuePair<string, Object> item in param)
                            {
                                SqlDbType type = SqlDbType.VarChar;

                                switch (item.Value.GetType().Name)
                                {
                                    case nameof(System.String):
                                        type = SqlDbType.VarChar;
                                        break;
                                    case nameof(DataTable):
                                        type = SqlDbType.Structured;
                                        break;
                                }

                                cmd.Parameters.Add(item.Key, type);
                                cmd.Parameters[0].Value = item.Value;
                            }
                        }

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                LogHelper.WriteLog($"{spName}, executed");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"{spName}, Exception: {ex.Message}");
                return false;
            }
        }
    }
}
