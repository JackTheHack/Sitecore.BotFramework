using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Dialogs;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class Prompt : IPromptDialogAction
    {
        [NonSerialized]
        private readonly Item _item;
        
        private IDialog _dialog;
        public bool IsPromptDialog => true;

        public Prompt(Item item)
        {
            _item = item;
        }

        public Task Execute(DialogActionContext context, ResumeAfter<object> resumeAction)
        {
            _dialog = context.Dialog;
            context.Context.Call(new SitecorePromptDialog(_item), resumeAction);
            return Task.CompletedTask;
        }

        public async Task HandleDialogResult(IDialogContext context, object dialogResult)
        {
            await context.PostAsync("TODO: Handle response - " + dialogResult);
        }
    }
}