using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IMerchantListsHandler
    {
        //Get
        Task<ResponseAccessTrade<MerchantListModel>> GetAllAsync();
        Task<ResponseObject<List<MerchantListModel>>> GetAllMerchantAsync();
        Task<ResponseObject<MerchantListModel>> GetMerchantByLoginNameAsync(string loginName);
        Task<ResponseObject<MerchantListModel>> UpdateAsync(MerchantListModel model);
        Task<ResponseObject<MerchantListModel>> UpdateHideAllAsync();
    }
}
