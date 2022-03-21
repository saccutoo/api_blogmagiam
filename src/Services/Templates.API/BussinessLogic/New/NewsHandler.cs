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
    public class NewsHandler : INewsHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<NewsHandler> _logger;
        public NewsHandler(ILogger<NewsHandler> logger = null, 
                                            IRedisService redisService=null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
           // _templatesHandler = templatesHandler;
        }




        #region "Select Default Table"

        public async Task<ResponseObject<List<NewsModel>>> GetTopNewByFilterAsync(NewsQuery model)
        {
            try
            {
                ResponseObject<List<NewsModel>> response = new ResponseObject<List<NewsModel>>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_PAGE_INDEX"; param.Value = model.pageIndex; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_PAGE_SIZE"; param.Value = model.pageSize; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_VALUE"; param.Value = model.value; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_STATUS"; param.Value = model.status; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TOTAL"; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_TOP_BY_FILTER", parameters,true);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<NewsModel>>(new List<NewsModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<List<NewsModel>>> GetTopNewByTypeAsync(int type)
        {
            try
            {
                ResponseObject<List<NewsModel>> response = new ResponseObject<List<NewsModel>>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "p_type"; param.Value = type; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetDatas("GET_TOP_NEW_BY_TYPE", parameters);
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<List<NewsModel>>(new List<NewsModel>(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }
      

        public async Task<ResponseObject<NewsModel>> GetNewByIdAsync(int id)
        {
            try
            {
                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_NEW_BY_ID", parameters);
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

        public async Task<ResponseObject<NewsModel>> GetNewByCodeAsync(string code,int id=0)
        {
            try
            {
                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_CODE"; param.Value = code; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_NEW_BY_CODE", parameters);
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

        #region "CRUD Default Table"
        public async Task<ResponseObject<NewsModel>> AddChangeAsync(NewsModel model)
        {
            try
            {
                var data = await GetNewByCodeAsync(model.code.Trim().Replace(" ", "-"));
                if (data!=null && data.Data!=null)
                {
                    return new ResponseObject<NewsModel>(new NewsModel(), "This code already exists", StatusCode.Fail);
                }
                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CODE"; param.Value = model.code.Trim().Replace(" ","-"); param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TITLE"; param.Value = model.title; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CONTENT"; param.Value = model.content; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_IMAGE"; param.Value = model.image; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = model.type; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_MERCHANT"; param.Value = model.type_merchant; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_LINK"; param.Value = model.link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CREATE_BY"; param.Value = model.create_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                int pId = await callDatabse.Excute("ADD_NEW", parameters);
                if (pId<0)
                {
                    return new ResponseObject<NewsModel>(null, "Thất bại", StatusCode.Fail);
                }
                response.Data = new NewsModel() {id=pId };

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

        public async Task<ResponseObject<NewsModel>> UpdateChangeAsync(NewsModel model)
        {
            try
            {
                var data = await GetNewByCodeAsync(model.code.Trim().Replace(" ", "-"),Convert.ToInt32(model.id));
                if (data != null && data.Data != null)
                {
                    return new ResponseObject<NewsModel>(new NewsModel(), "This code already exists", StatusCode.Fail);
                }
                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter();
                param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = model.id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CODE"; param.Value = model.code.Trim().Replace(" ", "-"); param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TITLE"; param.Value = model.title; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_CONTENT"; param.Value = model.content; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_IMAGE"; param.Value = model.image; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_TYPE"; param.Value = model.type; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_MERCHANT"; param.Value = model.type_merchant; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_LINK"; param.Value = model.link; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_UPDATE_BY"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                int pId = await callDatabse.Excute("UPDATE_NEW", parameters);
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

        public async Task<ResponseObject<NewsModel>> DeleteChangeAsync(NewsModel model)
        {
            try
            {
                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);              
                param = new MySqlParameter(); param.ParameterName = "P_UPDATE_BY"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                int pId = await callDatabse.Excute("DELETE_NEW", parameters);
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

        public async Task<ResponseObject<NewsModel>> UpdateStatusAsync(NewsModel model)
        {
            try
            {
                ResponseObject<NewsModel> response = new ResponseObject<NewsModel>();
                CallDatabseMySql<NewsModel> callDatabse = new CallDatabseMySql<NewsModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_ID"; param.Value = model.id; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_STATUS"; param.Value = model.status; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_UPDATE_BY"; param.Value = model.update_by; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                param = new MySqlParameter(); param.ParameterName = "P_OUT_ID"; param.Value = 0; param.DbType = DbType.Int32; param.Direction = ParameterDirection.Output; parameters.Add(param);
                int pId = await callDatabse.Excute("UPDATE_STATUS_NEW", parameters);
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
