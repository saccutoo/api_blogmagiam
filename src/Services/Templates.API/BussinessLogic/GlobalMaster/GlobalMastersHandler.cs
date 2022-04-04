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
    public class GlobalMastersHandler : IGlobalMastersHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<GlobalMastersHandler> _logger;
        public GlobalMastersHandler(ILogger<GlobalMastersHandler> logger = null, 
                                IRedisService redisService=null
                              )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }


        

        #region "Select Default Table"
        public async Task<ResponseObject<List<GlobalMastersModel>>> GetGlobalMasterByParentCodeAsync(string code)
        {
            try
            {
                ResponseObject<List<GlobalMastersModel>> response = new ResponseObject<List<GlobalMastersModel>>();
                CallDatabseMySql<GlobalMastersModel> callDatabse = new CallDatabseMySql<GlobalMastersModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_CODE"; param.Value = code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_GLOBAL_MASTER_BY_PARENT_CODE", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<GlobalMastersModel>>(new List<GlobalMastersModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<GlobalMastersModel>> GetGlobalMasterByCodeAsync(string code)
        {
            try
            {
                ResponseObject<GlobalMastersModel> response = new ResponseObject<GlobalMastersModel>();
                CallDatabseMySql<GlobalMastersModel> callDatabse = new CallDatabseMySql<GlobalMastersModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_CODE"; param.Value = code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_GLOBAL_MASTER_BY_CODE", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<GlobalMastersModel>(new GlobalMastersModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<GlobalMastersModel>> GetGlobalMasterByIdAsync(int id)
        {
            try
            {
                ResponseObject<GlobalMastersModel> response = new ResponseObject<GlobalMastersModel>();
                CallDatabseMySql<GlobalMastersModel> callDatabse = new CallDatabseMySql<GlobalMastersModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("P_CODE", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<GlobalMastersModel>(new GlobalMastersModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        #endregion

        #region "CRUD Default Table"


        #endregion

    }
}
