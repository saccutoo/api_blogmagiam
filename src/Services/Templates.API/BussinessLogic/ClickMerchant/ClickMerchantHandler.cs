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
    public class ClickMerchantHandler : IClickMerchantHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<ClickMerchantHandler> _logger;
        public ClickMerchantHandler(ILogger<ClickMerchantHandler> logger = null, 
                                            IRedisService redisService=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }




        #region "Select Default Table"
        public async Task<ResponseObject<List<ClickMerchantModel>>> GetClickMerchantByFilterAsync(ClickMerchantQuery model)
        {
            try
            {
                ResponseObject<List<ClickMerchantModel>> response = new ResponseObject<List<ClickMerchantModel>>();
                CallDatabseMySql<ClickMerchantModel> callDatabse = new CallDatabseMySql<ClickMerchantModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_PAGE_INDEX"; param.Value = model.pageIndex; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_PAGE_SIZE"; param.Value = model.pageSize; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_VALUE"; param.Value = model.value; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = model.Type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_DATE_FIRST"; param.Value = model.FirstDate.Value; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_DATE_LAST"; param.Value = model.LastDate.Value; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TOTAL"; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_CLICK_MERCHANT_FILTER", parameters, true);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<ClickMerchantModel>>(new List<ClickMerchantModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        #endregion

        #region "CRUD Default Table"

        public async Task<ResponseObject<NewsModel>> AddChangeAsync(ClickMerchantModel model)
        {
            try
            {

                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter();
                param = new MySqlParameter(); param.ParameterName = "P_IP"; param.Value = model.ip; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_MERCHANT_NAME"; param.Value = model.merchant_name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_AFF_LINK"; param.Value = model.aff_link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                int pId = await callDatabse.Excute("ADD_CLICK_MERCHANT", parameters);
                if (pId < 0)
                {
                    return new ResponseObject<NewsModel>(null, "Thất bại", StatusCode.Fail);
                }
                response.Data = new NewsModel() { id = pId };

                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<NewsModel>(new NewsModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        #endregion

    }
}
