using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using SC90.Bot.Telegram.SchedulingJobs;

namespace SC90.Bot.Telegram.Services
{
    public class SchedulerService : ISchedulerService
    {
        private IScheduler _scheduler;
        private IJobDetail _schedulingJob;
        const string JobID = "botSchedulingJob";

        public async Task Initialize()
        {
            // construct a scheduler factory
            StdSchedulerFactory factory = new StdSchedulerFactory();

            _schedulingJob = 
                JobBuilder
                    .Create<BotSchedulerJob>()
                    .WithIdentity(new JobKey(JobID))
                    .StoreDurably(true)
                    .Build();

            _scheduler = await factory.GetScheduler();

            await _scheduler.AddJob(_schedulingJob, replace: true);
            await _scheduler.Start();
        }

        public async Task SetTimeout(string botName, string userName, string jobName, float seconds, SchedulingJobData jobData)
        {           
            var triggerKey = new TriggerKey(userName + "-" + jobData.JobId, botName);
            var existingTrigger = await _scheduler.GetTrigger(triggerKey);

            var trigger = TriggerBuilder.Create()
                .ForJob(JobID)
                .WithIdentity(triggerKey)
                .UsingJobData(jobData.ToJobData())
                .StartAt(DateTime.UtcNow.Add(TimeSpan.FromSeconds(seconds)))
                .Build();

            if (existingTrigger == null)
            {
                await _scheduler.ScheduleJob(trigger);
            }
            else
            {
                await _scheduler.RescheduleJob(triggerKey, trigger);
            }
        }

        public async Task SetInterval(string botName, string userName, string jobName, string cronExpression, SchedulingJobData jobData)
        {
            var triggerKey = new TriggerKey(userName + "-" + jobData.JobId, botName);
            var existingTrigger = await _scheduler.GetTrigger(triggerKey);

            var trigger = TriggerBuilder.Create()
                .ForJob(JobID)
                .WithIdentity(triggerKey)
                .UsingJobData(jobData.ToJobData())
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronExpression))
                .Build();

            if (existingTrigger == null)
            {
                await _scheduler.ScheduleJob(trigger);
            }
            else
            {
                await _scheduler.RescheduleJob(triggerKey, trigger);
            }
        }

        public async Task StopJob(string botName, string userName, string triggerName)
        {
            var triggerKey = new TriggerKey(userName + "-" + triggerName, botName);
            var trigger = await _scheduler.GetTrigger(triggerKey);

            if (trigger!= null)
            {
                await _scheduler.UnscheduleJob(triggerKey);                
            }
        }

        public async Task ClearJobs(string botName)
        {
            var triggers = await _scheduler.GetTriggersOfJob(new JobKey(JobID));
            if(triggers != null)
            {
                foreach(var trigger in triggers)
                {
                    if (trigger.Key.Group == botName)
                    {
                        await _scheduler.UnscheduleJob(trigger.Key);
                    }
                }
            }
        }
    }
}
