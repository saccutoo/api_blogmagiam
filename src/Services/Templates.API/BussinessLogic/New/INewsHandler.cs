using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface INewsHandler
    {
        Task<ResponseObject<List<NewsModel>>> GetTopNewByFilterAsync(NewsQuery model);
        Task<ResponseObject<List<NewsModel>>> GetTopNewByTypeAsync(int type);
        Task<ResponseObject<NewsModel>> GetNewByIdAsync(int id);
        Task<ResponseObject<NewsModel>> GetNewByCodeAsync(string code, int id = 0);

        Task<ResponseObject<NewsModel>> AddChangeAsync(NewsModel model);

        Task<ResponseObject<NewsModel>> UpdateChangeAsync(NewsModel model);

        Task<ResponseObject<NewsModel>> DeleteChangeAsync(NewsModel model);

        Task<ResponseObject<NewsModel>> UpdateStatusAsync(NewsModel model);
    }
}
