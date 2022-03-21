using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Templates.API
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        public Type JobType { get; }
        public string CronExpression { get; } = "0/5 * * * * ?";
    }
}
