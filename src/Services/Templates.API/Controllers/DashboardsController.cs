using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Templates.API.BussinessLogic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Templates.API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly IDashboardsHandler _DashboardsInterfaceHandler;

        public DashboardsController(IRedisService redisService, IDashboardsHandler DashboardsInterfaceHandler)
        {
            _redisService = redisService;
            _DashboardsInterfaceHandler = DashboardsInterfaceHandler;
        }


        [HttpGet]
        [Route("get_count_click_merchant")]
        [ProducesResponseType(typeof(ResponseObject<List<DashboardsModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<DashboardsModel>>> GetCountClickMerchantAsync(string type)
        {
            DayOfWeek currentDay = DateTime.Now.DayOfWeek;
            int daysTillCurrentDay = 0;
            if (currentDay==DayOfWeek.Sunday)
            {
                daysTillCurrentDay = 6;
            }
            else
            {
                daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            }
            DateTime currentWeekStartDate = DateTime.Now.AddDays(-daysTillCurrentDay);
            DateTime currentWeekEndDate = currentWeekStartDate.AddDays(6);
            var response = await _DashboardsInterfaceHandler.GetCountClickMerchantAsync(type,currentWeekStartDate, currentWeekEndDate);
            return response;
        }
    }
}