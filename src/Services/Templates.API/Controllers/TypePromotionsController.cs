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
    [Route("api/typepromotion")]
    [ApiController]
    [Authorize]
    public class TypePromotionsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly ITypePromotionsHandler _TypePromotionsInterfaceHandler;

        public TypePromotionsController(IRedisService redisService, ITypePromotionsHandler TypePromotionsInterfaceHandler)
        {
            _redisService = redisService;
            _TypePromotionsInterfaceHandler = TypePromotionsInterfaceHandler;
        }


        /// <summary>
        /// Đồng bộ merchant 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_all")]
        [ProducesResponseType(typeof(ResponseObject<List<TypePromotionsModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<TypePromotionsModel>>> GetAllTypePromotionsAsync()
        {
            var result = await _TypePromotionsInterfaceHandler.GetAllTypePromotionsAsync();
            return result;
        }
    }
}