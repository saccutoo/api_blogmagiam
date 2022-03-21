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
    public class SynchronizedHandler : ISynchronizedHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<SynchronizedHandler> _logger;
        private readonly IMerchantListsHandler _merchantListsInterfaceHandler;

        public SynchronizedHandler(ILogger<SynchronizedHandler> logger = null, 
                                            IRedisService redisService=null,
                                            IMerchantListsHandler merchantListsInterfaceHandler=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
            _merchantListsInterfaceHandler = merchantListsInterfaceHandler;
        }


        public async Task<ResponseObject<SynchronizedModel>> SynchronizedMerchant()
        {
            try
            {
                ResponseObject<SynchronizedModel> response = new ResponseObject<SynchronizedModel>();
                //CallDatabseMySql<SynchronizedModel> callDatabse = new CallDatabseMySql<SynchronizedModel>();
                //List<MySqlParameter> parameters = new List<MySqlParameter>();
                //MySqlParameter param = new MySqlParameter(); param.ParameterName = "LOGIN_NAME"; param.Value = loginName; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                //response = await callDatabse.GetData("GET_MERCHANT_BY_LOGIN_NAME", parameters);
                //return response;


                ResponseAccessTrade<MerchantListModel> resAccesTrade = new ResponseAccessTrade<MerchantListModel>();
                resAccesTrade = await _merchantListsInterfaceHandler.GetAllAsync();
                if (resAccesTrade != null && resAccesTrade.Data!=null && resAccesTrade.Data.Count>0)
                {
                    await _merchantListsInterfaceHandler.UpdateHideAllAsync();
                    foreach (var item in resAccesTrade.Data)
                    {
                        ResponseObject<MerchantListModel> data = await _merchantListsInterfaceHandler.UpdateAsync(item);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<SynchronizedModel>(new SynchronizedModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

    }
}
