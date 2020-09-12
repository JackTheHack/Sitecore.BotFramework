using System.Collections.Generic;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2.Actions;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.DependencyInjection;

namespace SC90.Bot.Telegram.Actions
{
    public class SetTimeoutAction : IDialogueAction
    {
        private SetTimeout _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ITelegramService _telegramService;
        private readonly ISchedulerService _schedulerService;

        private ChatbotActionContext _context;
        
        public SetTimeoutAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _telegramService = ServiceLocator.ServiceProvider.GetService<ITelegramService>();
            _schedulerService = ServiceLocator.ServiceProvider.GetService<ISchedulerService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<SetTimeout>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            var jobData = new SchedulingJobData
            {
                EventName = _actionItem.EventName,
                JobId = _actionItem.JobId,
                SessionId = _context.SessionKey,
                UserId = _context.ChatUpdate?.UserId,
                Source = _context.ChatUpdate?.Source,
                IsRecurrent = false,
                IsGlobal = _actionItem.IsGlobal,
                BotId = _context.Chatbot.Id,
                Parameters = new Dictionary<string, string>()
            };

            if (_actionItem.Parameters != null)
            {
                foreach (string actionItemParameter in _actionItem.Parameters)
                {
                    jobData.Parameters.Add(actionItemParameter, _actionItem.Parameters[actionItemParameter]);
                }
            }

            await _schedulerService.SetTimeout(
                _context.Chatbot.Id.ToString(), 
                _actionItem.IsGlobal ? string.Empty : _context.SessionKey, 
                _actionItem.JobId, 
                _actionItem.TimeoutSeconds, 
                jobData);
        }
    }
}
