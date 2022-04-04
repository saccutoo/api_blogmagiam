using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Templates.API.BussinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Templates.API.Controllers
{
    [Route("api/coupon_list")]
    [ApiController]
    [Authorize]

    public class CouponsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly ICouponsHandler _CouponsInterfaceHandler;
        private readonly ILogger<CouponsHandler> _logger;
        public CouponsController(IRedisService redisService, ICouponsHandler CouponsInterfaceHandler, ILogger<CouponsHandler> logger)
        {
            _redisService = redisService;
            _CouponsInterfaceHandler = CouponsInterfaceHandler;
            _logger = logger;
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


        /// <summary>
        /// Lấy toàn bộ thông tin được sử dụng Coupons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseAccessTrade<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<CouponModel>>> GetCouponByFilterAsync(string data)
        {
            CouponQuery model = JsonConvert.DeserializeObject<CouponQuery>(data);
            var result = await _CouponsInterfaceHandler.GetCouponByFilterAsync(model);
            return result;
        }

        [HttpGet]
        [Route("get_by_id")]
        [ProducesResponseType(typeof(ResponseObject<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<CouponModel>> GetNewByIdAsync(int id)
        {
            var result = await _CouponsInterfaceHandler.GetNewByIdAsync(id);
            return result;
        }
        #endregion

        #region "CRUD Default Table"
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<CouponModel>> AddChangeAsync(CouponModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.create_by = requestInfo.UserName;
            var result = await _CouponsInterfaceHandler.AddChangeAsync(model);
            return result;
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<CouponModel>> UpdateChangeAsync(CouponModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.update_by = requestInfo.UserName;
            var result = await _CouponsInterfaceHandler.UpdateChangeAsync(model);
            return result;
        }


        [HttpPut]
        [Route("update-status")]
        [ProducesResponseType(typeof(ResponseObject<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<CouponModel>> UpdateStatusAsync(CouponDeleteModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.update_by = requestInfo.UserName;
            var result = await _CouponsInterfaceHandler.UpdateStatusAsync(model);
            return result;
        }

        [HttpPut]
        [Route("update-order")]
        [ProducesResponseType(typeof(ResponseObject<CouponModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<Response>>> UpdateOderAsync(List<CouponModel> model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            foreach (var item in model)
            {
                item.update_by = requestInfo.UserName;
            }
            var result = await _CouponsInterfaceHandler.UpdateOrderAsync(model);
            return result;
        }
        #endregion

    }
}