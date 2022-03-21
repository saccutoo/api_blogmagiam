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
    public class MenusHandler : IMenusHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<MenusHandler> _logger;
        public MenusHandler(ILogger<MenusHandler> logger = null, 
                                            IRedisService redisService=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }


        

        #region "Select Default Table"
        public async Task<ResponseObject<List<MenusModel>>> GetAllMenusAsync(string type)
        {
            try
            {
                ResponseObject<List<MenusModel>> response = new ResponseObject<List<MenusModel>>();
                CallDatabseMySql<MenusModel> callDatabse = new CallDatabseMySql<MenusModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_ALL_MENU", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<MenusModel>>(new List<MenusModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<MenusModel>> GetAllMenusByIdAsync(int id)
        {
            try
            {
                ResponseObject<MenusModel> response = new ResponseObject<MenusModel>();
                CallDatabseMySql<MenusModel> callDatabse = new CallDatabseMySql<MenusModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_MENU_BY_ID", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<MenusModel>(new MenusModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }
        #endregion

        #region "CRUD Default Table"


        #endregion

    }
}
