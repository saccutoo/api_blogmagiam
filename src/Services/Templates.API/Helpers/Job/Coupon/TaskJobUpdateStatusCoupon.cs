using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Templates.API.BussinessLogic;

namespace Templates.API
{
    public class TaskJobUpdateStatusCoupon : IJob
    {
        private readonly ICouponsHandler _couponsHandler;
        private readonly ILogger<TaskJobSynchronizedCoupon> _logger;

        public TaskJobUpdateStatusCoupon(ICouponsHandler couponsHandler, ISynchronizedHandler SynchronizedInterfaceHandler, ILogger<TaskJobSynchronizedCoupon> logger)
        {
            _couponsHandler = couponsHandler;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                _couponsHandler.JobUpdateStatusAsync();
                _logger.LogInformation("TaskJobUpdateStatusCoupon job success" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Task.CompletedTask;
        }

    }

}
