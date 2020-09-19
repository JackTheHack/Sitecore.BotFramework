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
    public class ClearSessionAction : IDialogueAction
    {
        private ClearSession _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ISessionProvider _sessionProvider;

        
        private ChatbotActionContext _context;

        public ClearSessionAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _sessionProvider = ServiceLocator.ServiceProvider.GetService<ISessionProvider>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<ClearSession>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            var sessionDocument = await _sessionProvider.GetSessionDocumentAsync(_context.SessionKey);
            foreach (var sessionDocumentName in sessionDocument.Names.ToList())
            {
                if (sessionDocumentName.StartsWith(SessionConstants.VariablePrefix))
                {
                    sessionDocument.Remove(sessionDocumentName);
                }
            }

            await _sessionProvider.UpdateSessionDocument(_context.SessionKey, sessionDocument);

        }
    }
}
