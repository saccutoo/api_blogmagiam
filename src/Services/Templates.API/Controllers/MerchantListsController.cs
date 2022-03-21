using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Templates.API.BussinessLogic;
using Microsoft.AspNetCore.Authorization;

namespace Templates.API.Controllers
{
    [Route("api/merchant_list")]
    [ApiController]
    [Authorize]
    public class MerchantListsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly IMerchantListsHandler _merchantListsInterfaceHandler;

        public MerchantListsController(IRedisService redisService, IMerchantListsHandler merchantListsInterfaceHandler)
        {
            _redisService = redisService;
            _merchantListsInterfaceHandler = merchantListsInterfaceHandler;
        }


        #region "Call api accesstrade"
        /// <summary>
        /// Lấy toàn bộ thông tin được sử dụng MerchantLists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseAccessTrade<MerchantListModel>), StatusCodes.Status200OK)]
        public async Task<ResponseAccessTrade<MerchantListModel>> GetAllAsync()
        {
            var result = await _merchantListsInterfaceHandler.GetAllAsync();
            return result;
        }


        #endregion


        #region "select database"
        /// <summary>
        /// Lấy toàn bộ thông tin được sử dụng MerchantLists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_all_merchant")]
        [ProducesResponseType(typeof(ResponseObject<List<MerchantListModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<MerchantListModel>>> GetAllMerchant()
        {
            var result = await _merchantListsInterfaceHandler.GetAllMerchantAsync();
            return result;
        }

        [HttpGet]
        [Route("get_merchant_by_loginname")]
        [ProducesResponseType(typeof(ResponseObject<MerchantListModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<MerchantListModel>> GetMerchantByLoginNameAsync(string loginName)
        {
            var result = await _merchantListsInterfaceHandler.GetMerchantByLoginNameAsync(loginName);
            return result;
        }
        #endregion

    }
}