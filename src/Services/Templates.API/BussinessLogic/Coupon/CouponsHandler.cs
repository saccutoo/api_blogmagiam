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
using MySql.Data.MySqlClient;
using System.Globalization;

namespace Templates.API.BussinessLogic
{
    public class CouponsHandler : ICouponsHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<CouponsHandler> _logger;
        private readonly IGlobalMastersHandler _globalMasterHander;
        public CouponsHandler(ILogger<CouponsHandler> logger = null,
                                            IRedisService redisService = null,
                                            IGlobalMastersHandler globalMasterHander = null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
            _globalMasterHander = globalMasterHander;
            // _templatesHandler = templatesHandler;
        }


        #region "Select Default Table"
        public async Task<ResponseAccessTrade<CouponModel>> GetByMerchantAsync(bool is_next_day_coupon, string keyword, string merchant, long limit, long page, string marchant)
        {
            try
            {
                ResponseAccessTrade<CouponModel> response = new ResponseAccessTrade<CouponModel>();
                var url = Helpers.GetConfig("API_ACCESSTRADE:COUPON_LIST");
                url = url + "?is_next_day_coupon=" + is_next_day_coupon + "&keyword=" + keyword + "&merchant=" + merchant + "&limit=" + limit + "&page=" + page;
                if (marchant == Helpers.GetConfig("LAZADA_TITLE"))
                {
                    ResponseAccessTrade<CouponLazadaModel> res = new ResponseAccessTrade<CouponLazadaModel>();
                    CallApi<CouponLazadaModel> callApi = new CallApi<CouponLazadaModel>();
                    res = await callApi.GetApi(url);
                    if (res.StatusCode == StatusCode.Success && res.Data != null && res.Data.Count > 0)
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
                        response.Data = JsonConvert.DeserializeObject<List<CouponModel>>(content1);
                        foreach (var item in response.Data)
                        {
                            string keywordFind = datas2.Where(s => s.id == item.id).FirstOrDefault().keyword;
                            item.keyword = new List<string>();
                            item.keyword.Add(keywordFind);
                        }
                    }
                }
                else if (marchant == Helpers.GetConfig("Shopee:MerchantName"))
                {
                    var responseShopeeKeyword = await _globalMasterHander.GetGlobalMasterByCodeAsync(Constants.ShopeeKeyCode);
                    if (responseShopeeKeyword != null && responseShopeeKeyword.Data != null && responseShopeeKeyword.Data.id != 0)
                    {
                        keyword = responseShopeeKeyword.Data.value;
                    }
                    else
                    {
                        keyword = string.Empty;
                    }
                    url = Helpers.GetConfig("API_ACCESSTRADE:COUPON_LIST");
                    url = url + "?is_next_day_coupon=" + is_next_day_coupon + "&keyword=" + keyword + "&merchant=" + merchant + "&limit=" + limit + "&page=" + page;
                    CallApi<CouponModel> callApi = new CallApi<CouponModel>();
                    response = await callApi.GetApi(url);
                    //if (response==null || response.Data==null || response.Data.Count==0)
                    //{
                    //    url = Helpers.GetConfig("API_ACCESSTRADE:COUPON_LIST");
                    //    url = url + "?is_next_day_coupon=" + is_next_day_coupon + "&keyword=" + keyword + "&merchant=" + merchant + "&limit=" + limit + "&page=" + page;
                    //    keyword = Helpers.GetConfig("Shopee:Keyword2");
                    //    response = await callApi.GetApi(url);
                    //}
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
                    return new ResponseAccessTrade<CouponModel>(null, StatusCode.Fail, ex.Message);
                }
                else throw ex;
            }
        }

        public async Task<ResponseAccessTrade<CouponModel>> GetCouponFromAcessTradeByMerchantAsync(bool is_next_day_coupon, string keyword, string merchant, long limit, long page, string marchant)
        {
            try
            {
                ResponseAccessTrade<CouponModel> response = new ResponseAccessTrade<CouponModel>();
                var url = Helpers.GetConfig("API_ACCESSTRADE:COUPON_LIST");
                url = url + "?is_next_day_coupon=" + is_next_day_coupon + "&keyword=" + keyword + "&merchant=" + merchant + "&limit=" + limit + "&page=" + page;
                if (marchant == Helpers.GetConfig("LAZADA_TITLE"))
                {
                    ResponseAccessTrade<CouponLazadaModel> res = new ResponseAccessTrade<CouponLazadaModel>();
                    CallApi<CouponLazadaModel> callApi = new CallApi<CouponLazadaModel>();
                    res = await callApi.GetApi(url);
                    if (res.StatusCode == StatusCode.Success && res.Data != null && res.Data.Count > 0)
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
                        response.Data = JsonConvert.DeserializeObject<List<CouponModel>>(content1);
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
                    return new ResponseAccessTrade<CouponModel>(null, StatusCode.Fail, ex.Message);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<List<CouponModel>>> GetCouponByMerchantAsync(string merchant)
        {
            try
            {
                ResponseObject<List<CouponModel>> response = new ResponseObject<List<CouponModel>>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_MERCHANT"; param.Value = merchant; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_COUPON_ACTIVE_BY_MERCHANT", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<CouponModel>>(new List<CouponModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }


        public async Task<ResponseObject<List<CouponModel>>> GetCouponByFilterAsync(CouponQuery model)
        {
            try
            {
                ResponseObject<List<CouponModel>> response = new ResponseObject<List<CouponModel>>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_PAGE_INDEX"; param.Value = model.pageIndex; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_PAGE_SIZE"; param.Value = model.pageSize; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_VALUE"; param.Value = model.value; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_MERCHANT"; param.Value = model.merchant; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = model.type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_STATUS"; param.Value = model.status; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TOTAL"; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_COUPON_BY_FILTER", parameters, true);

                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    response.Count = Convert.ToInt32(response.OutData["P_TOTAL"]);
                    foreach (var item in response.Data)
                    {
                        item.time_left = GetTimeLeft(item);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<CouponModel>>(new List<CouponModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<CouponModel>> GetNewByIdAsync(int id)
        {
            try
            {
                ResponseObject<CouponModel> response = new ResponseObject<CouponModel>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_COUPON_BY_ID", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<CouponModel>(new CouponModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public string GetTimeLeft(CouponModel item)
        {
            var hour = item.start_date.Hour.ToString();
            var minute = item.start_date.Minute.ToString();

            if (item.start_date.Hour < 10)
            {
                hour = "0" + hour.ToString();
            }
            if (item.start_date.Minute < 10)
            {
                minute = "0" + minute.ToString();
            }
            if (item.start_date > DateTime.Now)
            {
                item.time_left = "Hiệu lực từ : " + hour + "h" + minute + " ngày " + item.start_date.ToString("dd/MM/yyyy");
            }
            else if (item.end_date < DateTime.Now)
            {
                item.time_left = "Hết hiệu lực";
            }
            else
            {
                var date = item.end_date - DateTime.Now;
                hour = date.Hours.ToString();
                minute = date.Minutes.ToString();
                if (date.Hours < 10)
                {
                    hour = "0" + hour.ToString();
                }
                if (date.Minutes < 10)
                {
                    minute = "0" + minute.ToString();
                }
                if (date.Days > 0)
                {
                    item.time_left = "Còn " + date.Days.ToString() + " ngày ";
                }
                if (date.Days == 0)
                {
                    item.time_left = "Còn " + hour + " giờ " + minute + " phút";
                }
            }

            return item.time_left;
        }
        #endregion

        #region CRUD
        public async Task<ResponseObject<CouponModel>> SynchronizedCouponDataAsync()
        {
            try
            {
                ResponseObject<CouponModel> response = new ResponseObject<CouponModel>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                MerchantListsHandler merchantListsHandler = new MerchantListsHandler();
                var responseMerchant = await merchantListsHandler.GetAllMerchantAsync();
                if (responseMerchant != null && responseMerchant.Data != null && responseMerchant.Data.Count > 0)
                {
                    foreach (var merchant in responseMerchant.Data)
                    {
                        int pageIndex = 1;
                        double totalPageNewToFixed = 0;

                        //Lấy danh sách mã đang chạy
                        var responseCouponAccessTrade = await GetCouponFromAcessTradeByMerchantAsync(false, string.Empty, merchant.merchant_id, int.MaxValue, pageIndex, merchant.login_name);
                        if (responseCouponAccessTrade != null && responseCouponAccessTrade.Data != null && responseCouponAccessTrade.Data.Count > 0)
                        {
                            //totalPageNewToFixed = Math.Ceiling((double)responseCouponAccessTrade.Count / (double)responseCouponAccessTrade.Data.Count);
                            foreach (var item in responseCouponAccessTrade.Data.Where(s => s.shop_id == 0))
                            {
                                CouponModel model = item;
                                model.order = Constants.Limit;
                                model.start_date = model.start_time;
                                model.end_date = model.end_time;
                                model.type = Constants.Coupon_AccessTrade;
                                model.coupon_code = model.coupons[0].coupon_code;
                                model.create_by = Constants.Job;
                                model.update_by = Constants.Job;
                                List<MySqlParameter> parameters = new List<MySqlParameter>();
                                MySqlParameter param = new MySqlParameter(); param.ParameterName = "p_coupon_accesstrade_id"; param.Value = model.id; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_aff_link"; param.Value = model.aff_link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_image"; param.Value = model.image; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_name"; param.Value = model.name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_merchant"; param.Value = model.merchant; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_coupon_code"; param.Value = model.coupon_code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_content"; param.Value = model.content; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_remain"; param.Value = model.remain; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_start_date"; param.Value = model.start_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_end_date"; param.Value = model.end_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_order"; param.Value = model.order; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_type"; param.Value = model.type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_create_by"; param.Value = model.create_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_update_by"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                await callDatabse.Excute("SYNCHRONIZED_COUPON_DATA", parameters);
                            }
                        }

                        //Lấy danh sách mã sắp mở
                        var responseCouponAccessTradeNew = await GetCouponFromAcessTradeByMerchantAsync(true, string.Empty, merchant.merchant_id, int.MaxValue, pageIndex, merchant.login_name);
                        if (responseCouponAccessTradeNew != null && responseCouponAccessTradeNew.Data != null && responseCouponAccessTradeNew.Data.Count > 0)
                        {
                            //totalPageNewToFixed = Math.Ceiling((double)responseCouponAccessTrade.Count / (double)responseCouponAccessTrade.Data.Count);
                            foreach (var item in responseCouponAccessTradeNew.Data.Where(s => s.shop_id == 0))
                            {
                                CouponModel model = item;
                                model.order = Constants.Limit;
                                model.start_date = model.start_time;
                                model.end_date = model.end_time;
                                model.type = Constants.Coupon_AccessTrade;
                                model.coupon_code = model.coupons[0].coupon_code;
                                model.create_by = Constants.Job;
                                model.update_by = Constants.Job;
                                List<MySqlParameter> parameters = new List<MySqlParameter>();
                                MySqlParameter param = new MySqlParameter(); param.ParameterName = "p_coupon_accesstrade_id"; param.Value = model.id; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_aff_link"; param.Value = model.aff_link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_image"; param.Value = model.image; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_name"; param.Value = model.name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_merchant"; param.Value = model.merchant; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_coupon_code"; param.Value = model.coupon_code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_content"; param.Value = model.content; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_remain"; param.Value = model.remain; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_start_date"; param.Value = model.start_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_end_date"; param.Value = model.end_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_order"; param.Value = model.order; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_type"; param.Value = model.type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_create_by"; param.Value = model.create_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                param = new MySqlParameter(); param.ParameterName = "p_update_by"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                                await callDatabse.Excute("SYNCHRONIZED_COUPON_DATA", parameters);
                            }
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<CouponModel>(new CouponModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<CouponModel>> AddChangeAsync(CouponModel model)
        {
            try
            {
                ResponseObject<CouponModel> response = new ResponseObject<CouponModel>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                model.start_date = DateTime.ParseExact(model.start_date_string, Helpers.GetConfig("DATE:FORMAT_DATETIME"), CultureInfo.InvariantCulture);
                model.end_date = DateTime.ParseExact(model.end_date_string, Helpers.GetConfig("DATE:FORMAT_DATETIME"), CultureInfo.InvariantCulture);
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_AFF_LINK"; param.Value = model.aff_link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_IMAGE"; param.Value = model.image; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_NAME"; param.Value = model.name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_MERCHANT"; param.Value = model.merchant; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_COUPON_CODE"; param.Value = model.coupon_code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CONTENT"; param.Value = model.content; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_REMAIN"; param.Value = model.remain; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_START_DATE"; param.Value = model.start_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_END_DATE"; param.Value = model.end_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_ORDER"; param.Value = model.order; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = model.type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CREATE_BY"; param.Value = model.create_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                int pId = await callDatabse.Excute("ADD_COUPON", parameters);
                if (pId < 0)
                {
                    return new ResponseObject<CouponModel>(null, "Thất bại", StatusCode.Fail);
                }
                response.Data = new CouponModel() { id = pId.ToString() };

                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<CouponModel>(new CouponModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<CouponModel>> UpdateChangeAsync(CouponModel model)
        {
            try
            {
                ResponseObject<CouponModel> response = new ResponseObject<CouponModel>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter();
                model.start_date = DateTime.ParseExact(model.start_date_string, Helpers.GetConfig("DATE:FORMAT_DATETIME"), CultureInfo.InvariantCulture);
                model.end_date = DateTime.ParseExact(model.end_date_string, Helpers.GetConfig("DATE:FORMAT_DATETIME"), CultureInfo.InvariantCulture);
                param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = model.id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_AFF_LINK"; param.Value = model.aff_link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_IMAGE"; param.Value = model.image; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_NAME"; param.Value = model.name; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_MERCHANT"; param.Value = model.merchant; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_COUPON_CODE"; param.Value = model.coupon_code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CONTENT"; param.Value = model.content; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_REMAIN"; param.Value = model.remain; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_START_DATE"; param.Value = model.start_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_END_DATE"; param.Value = model.end_date; param.DbType = DbType.DateTime; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_ORDER"; param.Value = model.order; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = model.type; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_UPDATE_BY"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                int pId = await callDatabse.Excute("UPDATE_COUPON", parameters);
                if (pId < 0)
                {
                    return new ResponseObject<CouponModel>(null, "Thất bại", StatusCode.Fail);
                }
                response.Data = new CouponModel() { id = pId.ToString() };

                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<CouponModel>(new CouponModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<CouponModel>> UpdateStatusAsync(CouponDeleteModel model)
        {
            try
            {
                ResponseObject<CouponModel> response = new ResponseObject<CouponModel>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = Convert.ToInt32(model.id); param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_STATUS"; param.Value = model.status; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_UPDATE_BY"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                int pId = await callDatabse.Excute("UPDATE_STATUS_COUPON", parameters);
                if (pId < 0)
                {
                    return new ResponseObject<CouponModel>(null, "Thất bại", StatusCode.Fail);
                }
                response.Data = new CouponModel() { id = pId.ToString() };

                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<CouponModel>(new CouponModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<List<Response>>> UpdateOrderAsync(List<CouponModel> model)
        {
            ResponseObject<List<Response>> response = new ResponseObject<List<Response>>();
            response.Data = new List<Response>();
            try
            {
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                foreach (var item in model)
                {
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = Convert.ToInt32(item.id); param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                    param = new MySqlParameter(); param.ParameterName = "P_ORDER"; param.Value = item.order; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                    param = new MySqlParameter(); param.ParameterName = "P_UPDATE_BY"; param.Value = item.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                    param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                    int pId = await callDatabse.Excute("UPDATE_ORDER_COUPON", parameters);
                    if (pId < 0)
                    {
                        response.Data.Add(new Response()
                        {
                            StatusCode = StatusCode.Fail,
                            Message = item.id + " thất bại"
                        });
                    }
                    else
                    {
                        response.Data.Add(new Response()
                        {
                            StatusCode = StatusCode.Success,
                            Message = item.id + " thành công"
                        });
                    }
                }


                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    response.Data.Add(new Response()
                    {
                        StatusCode = StatusCode.Fail,
                        Message = "Exception Error: " + ex.Message
                    });
                    response.StatusCode = StatusCode.Fail;
                    response.Message = "Exception Error: " + ex.Message;
                    return response;
                }
                else throw ex;
            }
        }


        public async Task<ResponseObject<CouponModel>> JobUpdateStatusAsync()
        {
            try
            {
                ResponseObject<List<CouponModel>> response = new ResponseObject<List<CouponModel>>();
                CallDatabseMySql<CouponModel> callDatabse = new CallDatabseMySql<CouponModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                response = await callDatabse.GetDatas("GET_COUPON_EXPIRE", parameters, true);
                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    foreach (var item in response.Data)
                    {
                        CouponDeleteModel model = new CouponDeleteModel()
                        {
                            id = item.id,
                            status = 2,
                            update_by = "JOB"
                        };
                        await UpdateStatusAsync(model);
                    }
                }
                return new ResponseObject<CouponModel>(new CouponModel(), "Thành công", StatusCode.Success);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<CouponModel>(new CouponModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }
        #endregion

    }

}
