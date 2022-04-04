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
    public class TaskJobSynchronizedMerchant : IJob
    {
        private readonly ISynchronizedHandler _SynchronizedInterfaceHandler;
        private readonly ILogger<TaskJobSynchronizedMerchant> _logger;

        public TaskJobSynchronizedMerchant(ISynchronizedHandler SynchronizedInterfaceHandler, ILogger<TaskJobSynchronizedMerchant> logger)
        {
            _SynchronizedInterfaceHandler = SynchronizedInterfaceHandler;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                _SynchronizedInterfaceHandler.SynchronizedMerchant();
                _logger.LogInformation("TaskJobSynchronizedMerchant job success" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            return Task.CompletedTask;
        }
       
    }
    
}
