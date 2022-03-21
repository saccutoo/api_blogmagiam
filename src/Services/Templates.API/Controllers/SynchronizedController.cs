using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Templates.API.BussinessLogic;

namespace Templates.API.Controllers
{
    [Route("api/synchronized")]
    [ApiController]

    public class SynchronizedController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly ISynchronizedHandler _SynchronizedInterfaceHandler;

        public SynchronizedController(IRedisService redisService, ISynchronizedHandler SynchronizedInterfaceHandler)
        {
            _redisService = redisService;
            _SynchronizedInterfaceHandler = SynchronizedInterfaceHandler;
        }


        /// <summary>
        /// Đồng bộ merchant 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("merchant")]
        [ProducesResponseType(typeof(ResponseObject<SynchronizedModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<SynchronizedModel>> SynchronizedMerchant()
        {
            var result = await _SynchronizedInterfaceHandler.SynchronizedMerchant();
            return result;
        }

    }
}