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
    public class StatusHandler : IStatusHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<StatusHandler> _logger;
        public StatusHandler(ILogger<StatusHandler> logger = null, 
                                            IRedisService redisService=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }


        

        #region "Select Default Table"
        public async Task<ResponseObject<List<StatusModel>>> GetAllStatusAsync()
        {
            try
            {
                ResponseObject<List<StatusModel>> response = new ResponseObject<List<StatusModel>>();
                CallDatabseMySql<StatusModel> callDatabse = new CallDatabseMySql<StatusModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                response = await callDatabse.GetDatas("GET_ALL_STATUS", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<StatusModel>>(new List<StatusModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }


        #endregion

        #region "CRUD Default Table"
      

        #endregion

    }
}
