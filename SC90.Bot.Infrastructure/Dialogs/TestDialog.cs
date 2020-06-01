using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Sitecore.ChatBot.Dialogs
{
    [Serializable]
    public partial class TestDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (!context.PrivateConversationData.GetValueOrDefault("greetingDone", false))
            {
                context.PrivateConversationData.SetValue("greetingDone", true);
                await context.PostAsync("Hello, i'm smart and friendly Sitecore bot!");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
            context.Done("Bye.");
            }
        }

        protected virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            return Task.CompletedTask;
        }
    }
}