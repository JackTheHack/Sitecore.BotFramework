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
    public class StopJobAction : IDialogueAction
    {
        private StopJob _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ISchedulerService _schedulerService;

        private ChatbotActionContext _context;
        
        public StopJobAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _schedulerService = ServiceLocator.ServiceProvider.GetService<ISchedulerService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<StopJob>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            await _schedulerService.StopJob(
                _context.Chatbot.Id.ToString(), 
                _actionItem.IsGlobal ? string.Empty : _context.SessionKey,  
                _actionItem.JobId);
        }
    }
}
