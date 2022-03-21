using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic.Common
{
    public class CallDatabseMySql<T>
    {
        public string ConnectionString { get; set; }
        public CallDatabseMySql()
        {
            ConnectionString = Helpers.GetConfig("MY_SQL:ConnectionStringBlogMaGiam");
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public async Task<ResponseObject<List<T>>> GetDatas(string procedure, List<MySqlParameter> parameters, bool hasOutPut=false)
        {
            ResponseObject<List<T>> res = new ResponseObject<List<T>>();
            List<Dictionary<string, object>> listDic = new List<Dictionary<string, object>>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.FieldCount > 0)
                            {
                                Dictionary<string, object> dicObj = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var column = reader.GetName(i);
                                    var propVal = reader[column].ToString();
                                    if (string.IsNullOrEmpty(propVal)) propVal = null;
                                    dicObj[column] = propVal;
                                }
                                listDic.Add(dicObj);
                            }
                        }
                        if (listDic.Count > 0)
                        {
                            res.Data = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(listDic));
                        }
                    }
                    if (hasOutPut)
                    {
                        await cmd.ExecuteNonQueryAsync();
                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (var item in parameters)
                            {
                                if (item.Direction==ParameterDirection.Output)
                                {
                                    res.OutData.Add(item.ParameterName,item.Value?.ToString());
                                }
                            }
                        }
                    }

                }
                conn.Close();
            }
            return res;
        }

        public async Task<ResponseObject<T>> GetData(string procedure, List<MySqlParameter> parameters)
        {
            ResponseObject<T> res = new ResponseObject<T>();
            Dictionary<string, object> dicObj = new Dictionary<string, object>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.FieldCount > 0)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var column = reader.GetName(i);
                                    var propVal = reader[column].ToString();
                                    if (string.IsNullOrEmpty(propVal)) propVal = null;
                                    dicObj[column] = propVal;
                                }
                            }
                        }
                    }
                    if (dicObj.Count > 0)
                    {
                        res.Data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dicObj));
                    }
                }
                conn.Close();
            }
            return res;
        }

        public async Task<int> Excute(string procedure, List<MySqlParameter> parameters)
        {
            int id = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }

                    await cmd.ExecuteNonQueryAsync();
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            if (item.Direction == ParameterDirection.Output && item.ParameterName== "P_OUT_ID")
                            {
                                id = Convert.ToInt32(item.Value?.ToString());
                            }
                        }
                    }
                }
                conn.Close();
            }
            return id;
        }
    }
}
