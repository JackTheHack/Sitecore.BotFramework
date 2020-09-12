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
    public class SetContextItemAction : IDialogueAction
    {
        private SetContextItem _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ISessionProvider _sessionProvider;
        private readonly ITelegramService _telegramService;
        
        private ChatbotActionContext _context;

        public SetContextItemAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _sessionProvider = ServiceLocator.ServiceProvider.GetService<ISessionProvider>();
            _telegramService = ServiceLocator.ServiceProvider.GetService<ITelegramService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<SetContextItem>(action.Id);

            if (_actionItem.Item != Guid.Empty)
            {
                //go to datasource if specified
                _actionItem = _sitecoreContext.GetItem<SetContextItem>(_actionItem.Item);
            }

            _context = context;
        }

        public async Task Execute()
        {
            _sessionProvider.Set(_context.SessionKey, SessionConstants.Item, _actionItem.Item);
        }
    }
}
