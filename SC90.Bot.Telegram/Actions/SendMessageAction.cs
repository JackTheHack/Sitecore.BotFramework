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
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SC90.Bot.Telegram.Actions
{
    public class SendMessageAction : IDialogueAction
    {
        private SendMessage _actionItem;
        
        private readonly ISitecoreContext _sitecoreContext;
        private readonly ITelegramService _telegramService;
        
        private ChatbotActionContext _context;

        public SendMessageAction()
        {
            _sitecoreContext = ServiceLocator.ServiceProvider.GetService<ISitecoreContext>();
            _telegramService = ServiceLocator.ServiceProvider.GetService<ITelegramService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<SendMessage>(action.Id);

            if (_actionItem.Item != Guid.Empty)
            {
                //go to datasource if specified
                _actionItem = _sitecoreContext.GetItem<SendMessage>(_actionItem.Item);
            }

            _context = context;
        }

        public async Task Execute()
        {
            await _telegramService.Client.SendTextMessageAsync(
                new ChatId(_context.ChatUpdate.UserId), 
                _actionItem.Text,
                ParseMode.MarkdownV2);
        }
    }
}
