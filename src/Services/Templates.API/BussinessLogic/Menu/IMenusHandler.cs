using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IMenusHandler
    {
        Task<ResponseObject<List<MenusModel>>> GetAllMenusAsync(string type);
        Task<ResponseObject<MenusModel>> GetAllMenusByIdAsync(int id);
    }
}
