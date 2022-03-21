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
    [Route("api/status")]
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly IStatusHandler _StatusInterfaceHandler;

        public StatusController(IRedisService redisService, IStatusHandler StatusInterfaceHandler)
        {
            _redisService = redisService;
            _StatusInterfaceHandler = StatusInterfaceHandler;
        }


        /// <summary>
        /// Đồng bộ merchant 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_all")]
        [ProducesResponseType(typeof(ResponseObject<List<StatusModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<StatusModel>>> GetAllStatusAsync()
        {
            var result = await _StatusInterfaceHandler.GetAllStatusAsync();
            return result;
        }

    }
}