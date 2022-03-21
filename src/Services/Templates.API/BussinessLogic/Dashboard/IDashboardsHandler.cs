using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface IDashboardsHandler
    {
        Task<ResponseObject<List<DashboardsModel>>> GetCountClickMerchantAsync(string type,DateTime firstDateInWeek, DateTime lastDateInWeek);
    }
}
