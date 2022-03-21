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
    }
}
