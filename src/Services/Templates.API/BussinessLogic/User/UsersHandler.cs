using System.Threading.Tasks;
using Utils;
using API.Infrastructure.Repositories;
using Oracle.ManagedDataAccess.Client;
using Dapper.Oracle;
using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.BuildingBlocks.EventBus.Helpers;
using Templates.API.BussinessLogic;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;
using Templates.API.BussinessLogic.Common;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Templates.API.BussinessLogic
{
    public class UsersHandler : IUsersHandler
    {

        private readonly IRedisService _redisService;
        private readonly ILogger<UsersHandler> _logger;
        public UsersHandler(ILogger<UsersHandler> logger = null,
                                            IRedisService redisService = null
                                           )
        {
            _logger = logger;
            _redisService = redisService;
            // _templatesHandler = templatesHandler;
        }

        #region "Select Default Table"
        public async Task<ResponseObject<UsersModel>> GetTokenDefualtAsyncGetUserByUserNameAsync(string userName, string passWord)
        {
            try
            {
                ResponseObject<UsersModel> response = new ResponseObject<UsersModel>();
                CallDatabseMySql<UsersModel> callDatabse = new CallDatabseMySql<UsersModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_USERNAME"; param.Value = userName; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_USER_BY_USERNAME", parameters);
                if (response != null && response.Data != null)
                {
                    if (response.Data.block == 1)
                    {
                        return new ResponseObject<UsersModel>(new UsersModel(), "Tài khoản đã bị khóa!", StatusCode.Fail);
                    }
                    else if (response.Data.pass_word != passWord)
                    {
                        return new ResponseObject<UsersModel>(new UsersModel(), "Tài khoản hoặc mật khẩu không trùng khớp!", StatusCode.Fail);
                    }
                    else
                    {
                        response.Data.pass_word = string.Empty;
                        response.Data.token = GenerateJSONWebToken(Convert.ToInt32(Helpers.GetConfig("JWT:TimeExpire")));
                    }
                }
                else
                {
                    return new ResponseObject<UsersModel>(new UsersModel(), "Tài khoản hoặc mật khẩu không trùng khớp!", StatusCode.Fail);
                }
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<UsersModel>(new UsersModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<UsersModel>> GetUserByUserNameAsync(string userName, string passWord)
        {
            try
            {
                ResponseObject<UsersModel> response = new ResponseObject<UsersModel>();
                CallDatabseMySql<UsersModel> callDatabse = new CallDatabseMySql<UsersModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_USERNAME"; param.Value = userName; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_USER_BY_USERNAME", parameters);
                if (response != null && response.Data != null)
                {
                    if (response.Data.block == 1)
                    {
                        return new ResponseObject<UsersModel>(new UsersModel(), "Account has been locked!", StatusCode.Fail);
                    }
                    else if (response.Data.pass_word != passWord)
                    {
                        return new ResponseObject<UsersModel>(new UsersModel(), "Account or password does not match!", StatusCode.Fail);
                    }
                    else
                    {
                        response.Data.token = GenerateJSONWebToken(Convert.ToInt32(Helpers.GetConfig("JWT:TimeDefault")));
                    }
                }
                else
                {
                    return new ResponseObject<UsersModel>(new UsersModel(), "Account or password does not match!", StatusCode.Fail);
                }
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<UsersModel>(new UsersModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        public async Task<ResponseObject<UsersModel>> GetUserByUserNameNewAsync(string userName, string passWord)
        {
            try
            {
                ResponseObject<UsersModel> response = new ResponseObject<UsersModel>();
                CallDatabseMySql<UsersModel> callDatabse = new CallDatabseMySql<UsersModel>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter param = null;
                param = new MySqlParameter(); param.ParameterName = "P_USERNAME"; param.Value = userName; param.DbType = DbType.String; param.Direction = ParameterDirection.Input; parameters.Add(param);
                response = await callDatabse.GetData("GET_USER_BY_USERNAME", parameters);
                if (response != null && response.Data != null)
                {
                    if (response.Data.block == 1)
                    {
                        return new ResponseObject<UsersModel>(new UsersModel(), "Account has been locked!", StatusCode.Fail);
                    }
                    else if (response.Data.pass_word != passWord)
                    {
                        return new ResponseObject<UsersModel>(new UsersModel(), "Account or password does not match!", StatusCode.Fail);
                    }
                    else
                    {
                        UsersModel data = new UsersModel()
                        {
                            user_name = response.Data.user_name,
                            token = GenerateJSONWebToken(Convert.ToInt32(Helpers.GetConfig("JWT:TimeDefault")))
                        };
                        response.Data = data;
                    }
                }
                else
                {
                    return new ResponseObject<UsersModel>(new UsersModel(), "Account or password does not match!", StatusCode.Fail);
                }
                return response;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseObject<UsersModel>(new UsersModel(), ex.Message, StatusCode.Fail);
                }
                else throw ex;
            }
        }

        #endregion

        #region "CRUD Default Table"


        #endregion


        private string GenerateJSONWebToken(int time)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Helpers.GetConfig("JWT:Key")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Helpers.GetConfig("JWT:Issuer"),
                audience: Helpers.GetConfig("JWT:Audience"),
                expires: DateTime.Now.AddMinutes(time),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
