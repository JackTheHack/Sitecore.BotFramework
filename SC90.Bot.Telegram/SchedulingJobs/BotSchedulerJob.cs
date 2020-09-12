using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
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
            Log.Info($"Quartz > Running scheduling job - {context.Trigger.Key.Name}", this);

            var sitecoreBotService = ServiceLocator.ServiceProvider.GetService<ISitecoreBotService>();
            await sitecoreBotService.HandleSchedulingJob(context);

            
        }
    }
}
