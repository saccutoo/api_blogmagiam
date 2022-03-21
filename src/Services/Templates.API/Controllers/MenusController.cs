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
    [Route("api/menu")]
    [ApiController]
    [Authorize]
    public class MenusController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly IMenusHandler _MenusInterfaceHandler;

        public MenusController(IRedisService redisService, IMenusHandler MenusInterfaceHandler)
        {
            _redisService = redisService;
            _MenusInterfaceHandler = MenusInterfaceHandler;
        }


        [HttpGet]
        [Route("get_all")]
        [ProducesResponseType(typeof(ResponseObject<List<MenusModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<MenusModel>>> GetAllMenusAsync(string type)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var response = await _MenusInterfaceHandler.GetAllMenusAsync(type);
            return response;
        }

        [HttpGet]
        [Route("by_id")]
        [ProducesResponseType(typeof(ResponseObject<MenusModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<MenusModel>> GetCountClickMerchantAsync(int id)
        {
            var response = await _MenusInterfaceHandler.GetAllMenusByIdAsync(id);
            return response;
        }
    }
}