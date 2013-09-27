using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.Routing;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Data {
    public class DB {
        private string connectionString { get; set; }
        public string table { get; set; }
        public string primaryKeyField { get; set; }
        
        // constructor -- accepts connection string param
        public DB(string connectionString) {
            this.connectionString = connectionString;
        }

        public DataSet Run(string sql) {
            DataSet ret = new DataSet();
            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                //try {
                Boolean debug = false;
                try {
                    debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
                } catch (Exception xx) { }

                var sw = Stopwatch.StartNew();
                
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                using (SqlDataAdapter adp = new SqlDataAdapter(cmd)) {
                    adp.Fill(ret);
                }

                sw.Stop();

                if (debug && sw.ElapsedMilliseconds > 0)
                    HttpContext.Current.Response.Write("took "+ sw.ElapsedMilliseconds +"ms for: "+ sql + "<br/>");
                
                //}
                //catch (Exception xxx) { HttpContext.Current.Response.Write(xxx.Message); }
            }
            return ret;
        }
        public DataSet Run(string sql, List<SqlParameter> paramz) {
            DataSet ret = new DataSet();
            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                //try {
                Boolean debug = false;
                try {
                    debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
                } catch (Exception xx) { }

                var sw = Stopwatch.StartNew();

                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql, connection)) {
                    foreach (SqlParameter par in paramz)
                        cmd.Parameters.Add(par);
                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd)) {
                        adp.Fill(ret);
                    }
                }

                sw.Stop();

                if (debug && sw.ElapsedMilliseconds > 0)
                    HttpContext.Current.Response.Write("took " + sw.ElapsedMilliseconds + "ms for: " + sql + "<br/>");

                //}
                //catch (Exception xxx) { HttpContext.Current.Response.Write(xxx.Message); }
            }
            return ret;
        }
        
        public void Runo(string sql) {
            Boolean debug = false;
            try {
                debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
            } catch (Exception xx) { }

            var sw = Stopwatch.StartNew();

            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                //try {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection)) {
                        cmd.ExecuteNonQuery();
                    }
               // }
                //catch (Exception xxx) { HttpContext.Current.Response.Write(xxx.Message); }
            }

            sw.Stop();

            if (debug && sw.ElapsedMilliseconds > 0)
                HttpContext.Current.Response.Write("took " + sw.ElapsedMilliseconds + "ms for: " + sql + "<br/>");
        }
        public object RunScalar(string sql, List<SqlParameter> paramz) {
            Boolean debug = false;
            object rety = null;
            try {
                debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
            } catch (Exception xx) { }

            var sw = Stopwatch.StartNew();

            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                
                //try {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql +"; SELECT SCOPE_IDENTITY() as ident", connection)) {
                    try {
                        foreach (SqlParameter p in paramz)
                            cmd.Parameters.Add(p);
                    } catch (Exception xxxx) { }
                    rety = cmd.ExecuteScalar();
                }
                // }
                //catch (Exception xxx) { HttpContext.Current.Response.Write(xxx.Message); }
            }

            sw.Stop();

            if (debug && sw.ElapsedMilliseconds > 0)
                HttpContext.Current.Response.Write("took " + sw.ElapsedMilliseconds + "ms for: " + sql + "<br/>");

            return rety;
        }
        public object RunScalar(string sql) {
            return RunScalar(sql, null);
        }
        public void Runo(string sql, List<SqlParameter> paramz) {
            Boolean debug = false;
            try {
                debug = Convert.ToBoolean(ConfigurationManager.AppSettings["debug"]);
            } catch (Exception xx) { }

            var sw = Stopwatch.StartNew();

            using (SqlConnection connection = new SqlConnection(this.connectionString)) {
                //try {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql, connection)) {
                    foreach (SqlParameter p in paramz)
                        cmd.Parameters.Add(p);
                    cmd.ExecuteNonQuery();
                }
                // }
                //catch (Exception xxx) { HttpContext.Current.Response.Write(xxx.Message); }
            }

            sw.Stop();

            if (debug && sw.ElapsedMilliseconds > 0)
                HttpContext.Current.Response.Write("took " + sw.ElapsedMilliseconds + "ms for: " + sql + "<br/>");
        }

        public DataRow GetRecordByKey(string table, string primaryKey, string primaryKeyValue) {
            DataSet ds = this.Run("select * from " + table + " where " + primaryKey + " = " + primaryKeyValue);
            try {
                return ds.Tables[0].Rows[0];// ds.Tables[0].Rows[0];
            }
            catch (Exception xxx) {
                return null;
            }
        }

        public static DataSet RunQuery(string sql) {
            DB temp = new DB(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            return temp.Run(sql);
        }

        public static void RunOther(string sql) {
            DB temp = new DB(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            temp.Runo(sql);
        }

        public Boolean TableExists(string table) {
            try {
                DataSet ds = this.Run("select count(*) as instances from sysobjects where type = 'U' and name = '" + table + "'");
                if (ds.Tables[0].Rows[0]["instances"].ToString() != "0")
                    return true;
            }
            catch (Exception xx) { return false; }
            return false;
        }

        public DataRowCollection GetRows(DataSet ds) {
            try {
                return ds.Tables[0].Rows;
            } catch (Exception xxx) { }
            return null;
        }
        public DataRow GetTopRow(DataSet ds) {
            try {
                return ds.Tables[0].Rows[0];
            } catch (Exception xxx) { }
            return null;
        }
        public string Prepare(string obj) {
            return "'" + System.Web.HttpUtility.HtmlEncode(obj) + "'";
        }

        public DataSet RunProcedure(string procedureName, Dictionary<string, string> parameters) {
            try {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString)) {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand()) {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procedureName;
                        if (parameters != null)
                            foreach (KeyValuePair<string, string> pair in parameters)
                                cmd.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                            // fill the dataset with the data
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
            } catch (Exception xx) { }
            return null;
        }

        public DataSet RunStoredProcedure(string procedureName, List<SqlParameter> parameters) {
            try {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString)) {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand()) {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procedureName;
                        if (parameters != null)
                            foreach (SqlParameter pz in parameters)
                                cmd.Parameters.Add(pz);
                            //foreach (KeyValuePair<string, string> pair in parameters)
                              //  cmd.Parameters.Add(new SqlParameter(pair.Key, pair.Value));
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                            // fill the dataset with the data
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
            } catch (Exception xx) { }
            return null;
        }


        // data member get functions
        public static int gInt(object src) {
            try {
                return (int)src;
            } catch (Exception xxx) { }
            return 0;
        }
        public static string gString(object src) {
            try {
                return HttpContext.Current.Server.HtmlDecode((string)src);
            } catch (Exception xxx) { }
            return "";
        }
        public static Boolean gBoolean(object src) {
            try {
                return (Boolean)src;
            } catch (Exception xxx) { }
            return false;
        }
        public static DateTime gDateTime(object src) {
            try {
                return DateTime.Parse((string)src);
            } catch (Exception xxx) { }
            return DateTime.MinValue;
        }
        // non static 
        public int Int(object src) {
            try {
                return (int)src;
            } catch (Exception xxx) { }
            return 0;
        }
        public string String(object src) {
            try {
                return HttpContext.Current.Server.HtmlDecode((string)src);
            } catch (Exception xxx) { }
            return "";
        }
        public Boolean Boolean(object src) {
            try {
                return (Boolean)src;
            } catch (Exception xxx) { }
            return false;
        }
        public DateTime Date(object src) {
            try {
                return DateTime.Parse((string)src);
            } catch (Exception xxx) { }
            return DateTime.MinValue;
        }
        public string getParamByKey(string key) {
            try {
                RouteData r = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                return r.Values[key].ToString();
            } catch (Exception xxx) {
                try {
                    return HttpContext.Current.Request.QueryString[key].ToString();
                } catch (Exception xx) { }
                return "";
            }
        }
    }
}