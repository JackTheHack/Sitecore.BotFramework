using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Infrastructure.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class SendFile : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _dialogAction;
        private readonly string _message;

        public SendFile(Item dialogAction)
        {
            _dialogAction = dialogAction;
            _message = _dialogAction["Attachment"];
        }

        public bool IsPromptDialog => false;

        public Task Execute(DialogActionContext context)
        {
            Task.Run(async () => context.Context.PostAsync(_message)).Wait();
            return Task.CompletedTask;
        }
    }
}