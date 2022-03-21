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
    public class TypeNewsHandler : ITypeNewsHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<TypeNewsHandler> _logger;
        public TypeNewsHandler(ILogger<TypeNewsHandler> logger = null, 
                                IRedisService redisService=null
                              )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }


        

        #region "Select Default Table"
        public async Task<ResponseObject<List<TypeNewsModel>>> GetAllTypeNewsAsync()
        {
            try
            {
                ResponseObject<List<TypeNewsModel>> response = new ResponseObject<List<TypeNewsModel>>();
                CallDatabseMySql<TypeNewsModel> callDatabse = new CallDatabseMySql<TypeNewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                response = await callDatabse.GetDatas("GET_ALL_TYPE_NEW", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<TypeNewsModel>>(new List<TypeNewsModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }


        #endregion

        #region "CRUD Default Table"
      

        #endregion

    }
}
