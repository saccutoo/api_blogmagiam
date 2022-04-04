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
    public class MerchantListsHandler : IMerchantListsHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<MerchantListsHandler> _logger;
        public MerchantListsHandler(ILogger<MerchantListsHandler> logger = null, 
                                            IRedisService redisService=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }


        #region "app api accesstrade"
        public async Task<ResponseAccessTrade<MerchantListModel>> GetAllAsync()
        {
            try
            {

                var url = Helpers.GetConfig("API_ACCESSTRADE:MERCHANT_LIST");
                CallApi<MerchantListModel> callApi = new CallApi<MerchantListModel>();
                var response = await callApi.GetApi(url);
                //if (response.Data.Count>0)
                //{
                //    var responseMerchantDB = await GetAllMerchantAsync();
                //    if (responseMerchantDB.Data!=null && responseMerchantDB.Data.Count>0 && response.Data.Count< responseMerchantDB.Data.Count)
                //    {
                //        await UpdateHideAllAsync();
                //        foreach (var item in response.Data)
                //        {
                //            ResponseObject<MerchantListModel> data = await UpdateAsync(item);
                //        }
                //    }
                //}
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseAccessTrade<MerchantListModel>(null, StatusCode.Fail, ex.Message);
                }
                else throw ex;
            }
        }


        #endregion

        #region "Select Default Table"
        public async Task<ResponseObject<List<MerchantListModel>>> GetAllMerchantAsync()
        {
            try
            {
                ResponseObject<List<MerchantListModel>> response = new ResponseObject<List<MerchantListModel>>();
                CallDatabseMySql<MerchantListModel> callDatabse = new CallDatabseMySql<MerchantListModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                response = await callDatabse.GetDatas("GET_ALL_MERCHANT", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<MerchantListModel>>(new List<MerchantListModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<MerchantListModel>> GetMerchantByLoginNameAsync(string loginName)
        {
            try
            {
                ResponseObject<MerchantListModel> response = new ResponseObject<MerchantListModel>();
                CallDatabseMySql<MerchantListModel> callDatabse = new CallDatabseMySql<MerchantListModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "LOGIN_NAME"; param.Value = loginName; param.DbType = DbType.String;param.Direction= ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_MERCHANT_BY_LOGIN_NAME", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<MerchantListModel>(new MerchantListModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        #endregion

        #region "CRUD Default Table"
        public async Task<ResponseObject<MerchantListModel>> UpdateAsync(MerchantListModel model)
        {
            try
            {
                ResponseObject<MerchantListModel> response = new ResponseObject<MerchantListModel>();
                CallDatabseMySql<MerchantListModel> callDatabse = new CallDatabseMySql<MerchantListModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                MySqlParameter param = new MySqlParameter(); param.ParameterName = "p_merchant_id"; param.Value = model.Id; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                 param = new MySqlParameter(); param.ParameterName = "p_display_name"; param.Value = model.display_name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "p_login_name"; param.Value = model.login_name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "p_logo"; param.Value = model.logo; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "p_logo_coupon"; param.Value = model.logo_coupon; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("SYNCHRONIZED_MERCHANT_DATA", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<MerchantListModel>(new MerchantListModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<MerchantListModel>> UpdateHideAllAsync()
        {
            try
            {
                ResponseObject<MerchantListModel> response = new ResponseObject<MerchantListModel>();
                CallDatabseMySql<MerchantListModel> callDatabse = new CallDatabseMySql<MerchantListModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                MySqlParameter param = new MySqlParameter();
                response = await callDatabse.GetData("UPDATE_STATUS_ALL_MERCHANT", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<MerchantListModel>(new MerchantListModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        #endregion

    }
}
