using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IClickMerchantHandler
    {
        Task<ResponseObject<List<ClickMerchantModel>>> GetClickMerchantByFilterAsync(ClickMerchantQuery model);
        Task<ResponseObject<NewsModel>> AddChangeAsync(ClickMerchantModel model);
    }
}
