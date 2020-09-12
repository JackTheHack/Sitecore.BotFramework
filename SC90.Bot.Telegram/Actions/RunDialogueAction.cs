using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2.Dialogues;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.DependencyInjection;

namespace SC90.Bot.Telegram.Actions
{
    public class RunDialogueAction : IDialogueAction
    {
        private RunDialogue _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ITelegramService _telegramService;
        private readonly ISessionProvider _sessionProvider;

        
        private ChatbotActionContext _context;
        private IDialogActionFactory _actionFactory;

        public RunDialogueAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _actionFactory = ServiceLocator.ServiceProvider.GetService<IDialogActionFactory>();
            _sessionProvider = ServiceLocator.ServiceProvider.GetService<ISessionProvider>();
            _telegramService = ServiceLocator.ServiceProvider.GetService<ITelegramService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<RunDialogue>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            var dialogue = _sitecoreContext.GetItem<_Dialogue>(_actionItem.Dialog);

            if (dialogue != null)
            {
                //TODO: Run actions
            }

            //todo: set state
        }
    }
}
