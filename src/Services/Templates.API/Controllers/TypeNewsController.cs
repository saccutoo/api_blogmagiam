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
    [Route("api/typenews")]
    [ApiController]
    [Authorize]
    public class TypeNewsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly ITypeNewsHandler _TypeNewsInterfaceHandler;

        public TypeNewsController(IRedisService redisService, ITypeNewsHandler TypeNewsInterfaceHandler)
        {
            _redisService = redisService;
            _TypeNewsInterfaceHandler = TypeNewsInterfaceHandler;
        }


        /// <summary>
        /// Đồng bộ merchant 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_all")]
        [ProducesResponseType(typeof(ResponseObject<List<TypeNewsModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<TypeNewsModel>>> GetAllTypeNewsAsync()
        {
            var result = await _TypeNewsInterfaceHandler.GetAllTypeNewsAsync();
            return result;
        }
       
    }
}