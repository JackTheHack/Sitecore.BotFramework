using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2.Actions;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Constants;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SC90.Bot.Telegram.Actions
{
    public class ChangeStateAction : IDialogueAction
    {
        private ChangeState _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ITelegramService _telegramService;
        private readonly ISessionProvider _sessionProvider;

        
        private ChatbotActionContext _context;

        public ChangeStateAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _sessionProvider = ServiceLocator.ServiceProvider.GetService<ISessionProvider>();
            _telegramService = ServiceLocator.ServiceProvider.GetService<ITelegramService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<ChangeState>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            await _sessionProvider.Set(_context.SessionKey, SessionConstants.State, _actionItem.NewState);
        }
    }
}
