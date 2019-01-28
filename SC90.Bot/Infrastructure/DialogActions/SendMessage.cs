using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class SendMessage : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _dialogAction;
        private readonly string _message;

        public SendMessage(Item dialogAction)
        {
            _dialogAction = dialogAction;
            _message = _dialogAction["Text"];
        }

        public bool IsPromptDialog => false;

        public async Task Execute(DialogActionContext context)
        {
            await context.Context.PostAsync(_message);
        }
    }
}