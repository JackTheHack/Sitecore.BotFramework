using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.Telegram.Models;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ISchedulerService
    {
        Task Initialize();
        Task SetTimeout(string botName, string userName, string jobName, float seconds, SchedulingJobData jobData);
        Task SetInterval(string botName, string userName, string jobName, string cronExpression, SchedulingJobData jobData);
        Task StopJob(string botName, string userName, string jobName);
        Task ClearJobs(string botName);
    }
}
