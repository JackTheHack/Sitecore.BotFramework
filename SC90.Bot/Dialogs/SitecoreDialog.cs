using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SC90.Bot.Infrastructure;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace SC90.Bot.Dialogs
{
    [Serializable]
    public class SitecoreDialog : IDialog
    {
        private readonly ID _sitecoreItemId;
        [NonSerialized]
        private DialogSequenceEngine _dialogSequenceExecutor;

        public SitecoreDialog(ID sitecoreItemId)
        {
            _sitecoreItemId = sitecoreItemId;
            _dialogSequenceExecutor = new DialogSequenceEngine();
            _dialogSequenceExecutor.LoadActions(sitecoreItemId);
        }

        public async Task StartAsync(IDialogContext context)
        {
            Log.Info("SitecoreDialog - Start Async", this);

            context.PrivateConversationData.SetValue(
                "currentActionIndex", 0);
            context.PrivateConversationData.SetValue(
                "currentAction", _sitecoreItemId.ToString());

            context.Wait(MessageReceivedAsync);                        
        }

        protected virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            Log.Info("SitecoreDialog - MessageReceivedAsync", this);                        

            var currentActionId = context.PrivateConversationData.GetValueOrDefault("currentAction", string.Empty);

            _dialogSequenceExecutor = new DialogSequenceEngine();
            _dialogSequenceExecutor.LoadActions(ID.Parse(currentActionId));
            await _dialogSequenceExecutor.ContinueExecution(this, context);
        }
    }
}