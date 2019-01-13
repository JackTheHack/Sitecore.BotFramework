using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    public class SendMessage : IDialogAction
    {
        private Item _dialogAction;
        private string _message;

        public SendMessage(Item dialogAction)
        {
            _dialogAction = dialogAction;
            _message = _dialogAction["Text"];
        }

        public bool IsPromptDialog => false;

        public async Task Execute(IDialogContext context, IDialog currentDialog)
        {
            await context.PostAsync(_message);
        }
    }
}