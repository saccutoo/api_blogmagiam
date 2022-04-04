using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface ICouponsHandler
    {
        //Get
        Task<ResponseAccessTrade<CouponModel>> GetByMerchantAsync(bool is_next_day_coupon, string keyword, string merchant, long limit, long page, string marchant); 
        Task<ResponseObject<CouponModel>> SynchronizedCouponDataAsync();
        Task<ResponseObject<List<CouponModel>>> GetCouponByFilterAsync(CouponQuery model);
        Task<ResponseObject<CouponModel>> GetNewByIdAsync(int id);

        //CRUD
        Task<ResponseObject<CouponModel>> AddChangeAsync(CouponModel model);
        Task<ResponseObject<CouponModel>> UpdateChangeAsync(CouponModel model);
        Task<ResponseObject<CouponModel>> UpdateStatusAsync(CouponDeleteModel model);
        Task<ResponseObject<List<Response>>> UpdateOrderAsync(List<CouponModel> model);

        Task<ResponseObject<CouponModel>> JobUpdateStatusAsync();

    }
}
