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
    [Route("api/news")]
    [ApiController]
    [Authorize]
    public class NewsController : ControllerBase
    {

        private readonly IRedisService _redisService;
        private readonly INewsHandler _NewsInterfaceHandler;

        public NewsController(IRedisService redisService, INewsHandler NewsInterfaceHandler)
        {
            _redisService = redisService;
            _NewsInterfaceHandler = NewsInterfaceHandler;
        }

        [HttpGet]
        [Route("get_top_by_filter")]
        [ProducesResponseType(typeof(ResponseObject<List<NewsModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<NewsModel>>> GetTopNewByFilterAsync(string data)
        {
            NewsQuery model = JsonConvert.DeserializeObject<NewsQuery>(data);
            var result = await _NewsInterfaceHandler.GetTopNewByFilterAsync(model);
            return result;
        }

        [HttpGet]
        [Route("get_top_by_type")]
        [ProducesResponseType(typeof(ResponseObject<List<NewsModel>>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<List<NewsModel>>> GetTopNewByTypeAsync(int type)
        {
            var result = await _NewsInterfaceHandler.GetTopNewByTypeAsync(type);
            return result;
        }

        [HttpGet]
        [Route("get_new_by_id")]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> GetNewByIdAsync(int id)
        {
            var result = await _NewsInterfaceHandler.GetNewByIdAsync(id);
            return result;
        }
        [HttpGet]
        [Route("get_new_by_code")]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> GetNewByCodeAsync(string code)
        {
            var result = await _NewsInterfaceHandler.GetNewByCodeAsync(code);
            return result;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> AddChangeAsync(NewsModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.create_by = requestInfo.UserName;
            var result = await _NewsInterfaceHandler.AddChangeAsync(model);
            return result;
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> UpdateChangeAsync(NewsModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.update_by = requestInfo.UserName;
            var result = await _NewsInterfaceHandler.UpdateChangeAsync(model);
            return result;
        }

        [HttpPost]
        [Route("delete")]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> DeleteChangeAsync(NewsModel model)
        {
            var result = await _NewsInterfaceHandler.DeleteChangeAsync(model);
            return result;
        }

        [HttpPut]
        [Route("update-status")]
        [ProducesResponseType(typeof(ResponseObject<NewsModel>), StatusCodes.Status200OK)]
        public async Task<ResponseObject<NewsModel>> UpdateStatusAsync(NewsModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.update_by = requestInfo.UserName;
            var result = await _NewsInterfaceHandler.UpdateStatusAsync(model);
            return result;
        }
    }
}