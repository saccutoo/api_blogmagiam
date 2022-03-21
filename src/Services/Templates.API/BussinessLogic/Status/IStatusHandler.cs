using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IStatusHandler
    {
        Task<ResponseObject<List<StatusModel>>> GetAllStatusAsync();
    }
}
