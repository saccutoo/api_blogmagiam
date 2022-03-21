using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Templates.API.BussinessLogic;
using Templates.API;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Templates.API.Controllers
{
    [Route("api/clickmerchant")]
    [ApiController]
    [Authorize]
    public class ClickMerchantController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly IClickMerchantHandler _ClickMerchantInterfaceHandler;

        public ClickMerchantController(IRedisService redisService, IClickMerchantHandler ClickMerchantInterfaceHandler)
        {
            _redisService = redisService;
            _ClickMerchantInterfaceHandler = ClickMerchantInterfaceHandler;
        }


        [HttpGet]
        [Route("get_filter")]
        [ProducesResponseType(typeof(ResponseObject<List<ClickMerchantModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<ClickMerchantModel>>> GetClickMerchantByFilterAsync(string data)
        {
            ClickMerchantQuery model = JsonConvert.DeserializeObject<ClickMerchantQuery>(data);
            if (model.Type==Constants.DAY)
            {
                model.FirstDate = DateTime.Now;
                model.LastDate = DateTime.Now;
            }
            else if (model.Type == Constants.WEEK)
            {
                DayOfWeek currentDay = DateTime.Now.DayOfWeek;
                int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
                model.FirstDate = DateTime.Now.AddDays(-daysTillCurrentDay);
                model.LastDate = model.FirstDate.Value.AddDays(6);
            }
            else if (model.Type == Constants.MONTH)
            {
                var listDate = HelperCommon.GetListDateinMonth(DateTime.Now.Year, DateTime.Now.Month);
                model.FirstDate = listDate.FirstOrDefault();
                model.LastDate = listDate.LastOrDefault();
            }
            else if (model.Type == Constants.YEAR)
            {
                string year = DateTime.Now.Year.ToString();
                model.FirstDate = Convert.ToDateTime("01/01/" + year);
                model.LastDate = Convert.ToDateTime("12/31/" + year);
            }
            else
            {
                model.FirstDate = DateTime.Now;
                model.LastDate = DateTime.Now;
            }
            var result = await _ClickMerchantInterfaceHandler.GetClickMerchantByFilterAsync(model);
            return result;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> AddChangeAsync(ClickMerchantModel model)
        {
            var result = await _ClickMerchantInterfaceHandler.AddChangeAsync(model);
            return result;
        }
    }
}