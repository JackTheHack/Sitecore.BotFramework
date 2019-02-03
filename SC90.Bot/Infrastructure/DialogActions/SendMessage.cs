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

        public Task Execute(DialogActionContext context)
        {
            Task.Run(async () => context.Context.PostAsync(_message)).Wait();
            return Task.CompletedTask;
        }
    }
}