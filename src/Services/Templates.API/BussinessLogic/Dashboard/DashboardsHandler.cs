using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using Oracle.ManagedDataAccess.Client;
using Dapper.Oracle;
using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.BuildingBlocks.EventBus.Helpers;
using Templates.API.BussinessLogic;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;
using Templates.API.BussinessLogic.Common;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Templates.API.BussinessLogic
{
    public class DashboardsHandler : IDashboardsHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<DashboardsHandler> _logger;
        public DashboardsHandler(ILogger<DashboardsHandler> logger = null, 
                                IRedisService redisService=null
                              )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }


        

        #region "Select Default Table"
        public async Task<ResponseObject<List<DashboardsModel>>> GetCountClickMerchantAsync(string type,DateTime firstDateInWeek, DateTime lastDateInWeek)
        {
            try
            {
                ResponseObject<List<DashboardsModel>> response = new ResponseObject<List<DashboardsModel>>();
                CallDatabseMySql<DashboardsModel> callDatabse = new CallDatabseMySql<DashboardsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter();param.ParameterName = "P_DATE_FIRT_WEEK"; param.Value = firstDateInWeek; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_DATE_LAS_WEEK"; param.Value = lastDateInWeek; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_TOTAL"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_TOTAL_DAY"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_TOTAL_WEEK"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_TOTAL_MONTH"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_TOTAL_YEAR"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_COUNT_CLICK_MERCHANT", parameters,true);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<DashboardsModel>>(new List<DashboardsModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }


        #endregion

        #region "CRUD Default Table"
      

        #endregion

    }
}
