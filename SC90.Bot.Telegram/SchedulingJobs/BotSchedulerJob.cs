using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.DependencyInjection;
using Log = Sitecore.Diagnostics.Log;

namespace SC90.Bot.Telegram.SchedulingJobs
{
    public class BotSchedulerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var botContext = ServiceLocator.ServiceProvider.GetService<IBotRequestContext>();

            Log.Info($"Quartz > Running scheduling job - {context.Trigger.Key.Name}", this);

            var jobData = new SchedulingJobData(context.Trigger.JobDataMap);
            
            botContext.SetBotContext(jobData.BotId, true);

            var sitecoreBotService = ServiceLocator.ServiceProvider.GetService<ISitecoreBotService>();
            await sitecoreBotService.HandleSchedulingJob(jobData);

            botContext.SetBotContext(jobData.BotId, false);
        }
    }
}
