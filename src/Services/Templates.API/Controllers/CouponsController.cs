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
    [Route("api/coupon_list")]
    [ApiController]
    [Authorize]

    public class CouponsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly ICouponsHandler _CouponsInterfaceHandler;

        public CouponsController(IRedisService redisService, ICouponsHandler CouponsInterfaceHandler)
        {
            _redisService = redisService;
            _CouponsInterfaceHandler = CouponsInterfaceHandler;
        }


        #region "Select Default Table"
        /// <summary>
        /// Lấy toàn bộ thông tin được sử dụng Coupons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_by_merchant")]
        [ProducesResponseType(typeof(ResponseAccessTrade<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseAccessTrade<CouponModel>> GetByMerchantAsync(bool is_next_day_coupon, string keyword, string merchant, long limit, long page, string marchant)
        {
            var result = await _CouponsInterfaceHandler.GetByMerchantAsync(is_next_day_coupon, keyword, merchant, limit, page, marchant);
            return result;
        }
      
        #endregion


        
    }
}