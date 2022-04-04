using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IGlobalMastersHandler
    {
        Task<ResponseObject<List<GlobalMastersModel>>> GetGlobalMasterByParentCodeAsync(string code);
        Task<ResponseObject<GlobalMastersModel>> GetGlobalMasterByCodeAsync(string code);
        Task<ResponseObject<GlobalMastersModel>> GetGlobalMasterByIdAsync(int id);
    }
}
