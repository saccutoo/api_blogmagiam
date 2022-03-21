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
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly IUsersHandler _UsersInterfaceHandler;

        public AuthController(IRedisService redisService, IUsersHandler UsersInterfaceHandler)
        {
            _redisService = redisService;
            _UsersInterfaceHandler = UsersInterfaceHandler;
        }


        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(ResponseObject<UsersModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<UsersModel>> GetUserByUserNameAsync(string userName,string passWord)
        {
            var response = await _UsersInterfaceHandler.GetUserByUserNameAsync(userName, passWord);
            return response;
        }

        [HttpPost]
        [Route("login_web")]
        [ProducesResponseType(typeof(ResponseObject<UsersModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<UsersModel>> GetUserByUserNameNewAsync(string userName, string passWord)
        {
            var response = await _UsersInterfaceHandler.GetUserByUserNameNewAsync(userName, passWord);
            return response;
        }

        [HttpPost]
        [Route("login/token/defualt")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject<UsersModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<UsersModel>> GetTokenDefualtAsync()
        {
            var response = await _UsersInterfaceHandler.GetUserByUserNameAsync(Helpers.GetConfig("USER_DEFAULT:UserName"), Helpers.GetConfig("USER_DEFAULT:PassWord"));
            return response;
        }
    }
}