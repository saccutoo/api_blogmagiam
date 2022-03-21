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

namespace Templates.API.BussinessLogic
{
    public class CouponsHandler : ICouponsHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<CouponsHandler> _logger;
        public CouponsHandler(ILogger<CouponsHandler> logger = null, 
                                            IRedisService redisService=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }
      

        #region "Select Default Table"
        public async Task<ResponseAccessTrade<CouponModel>> GetByMerchantAsync(bool is_next_day_coupon,string keyword,string merchant,long limit,long page,string marchant)
        {
            try
            {
                ResponseAccessTrade<CouponModel> response = new ResponseAccessTrade<CouponModel>();
                var url = Helpers.GetConfig("API_ACCESSTRADE:COUPON_LIST");
                url = url + "?is_next_day_coupon="+ is_next_day_coupon +"&keyword="+keyword+"&merchant="+merchant+"&limit="+limit+"&page=" + page;
                if (marchant== Helpers.GetConfig("LAZADA_TITLE"))
                {
                    ResponseAccessTrade<CouponLazadaModel> res = new ResponseAccessTrade<CouponLazadaModel>();
                    CallApi<CouponLazadaModel> callApi = new CallApi<CouponLazadaModel>();
                    res = await callApi.GetApi(url);
                    if (res.StatusCode==StatusCode.Success && res.Data!=null && res.Data.Count>0)
                    {
                        response.Count = res.Count;
                        string content = JsonConvert.SerializeObject(res.Data);
                        var datas1 = JsonConvert.DeserializeObject<List<CouponLazadaModel>>(content);
                        var datas2 = JsonConvert.DeserializeObject<List<CouponLazadaModel>>(content);
                        foreach (var item in datas1)
                        {
                            item.keyword = null;
                        }
                        string content1 = JsonConvert.SerializeObject(datas1);
                        response.Data= JsonConvert.DeserializeObject<List<CouponModel>>(content1);
                        foreach (var item in response.Data)
                        {
                            string keywordFind = datas2.Where(s => s.id == item.id).FirstOrDefault().keyword;
                            item.keyword = new List<string>();
                            item.keyword.Add(keywordFind);
                        }
                    }
                }
                else
                {
                    CallApi<CouponModel> callApi = new CallApi<CouponModel>();
                    response = await callApi.GetApi(url);
                }

                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseAccessTrade<CouponModel>(null,StatusCode.Fail, ex.Message);
                }
                else throw ex;
            }
        }
      

        #endregion

    }
}
