using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.BussinessLogic
{
    public interface ISynchronizedHandler
    {
        //Get
        Task<ResponseObject<SynchronizedModel>> SynchronizedMerchant();
    }
}
