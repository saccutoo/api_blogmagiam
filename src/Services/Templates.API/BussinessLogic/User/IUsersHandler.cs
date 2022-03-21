using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IUsersHandler
    {
        Task<ResponseObject<UsersModel>> GetUserByUserNameAsync(string userName, string passWord);
        Task<ResponseObject<UsersModel>> GetUserByUserNameNewAsync(string userName, string passWord);
    }
}
