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
    public class SitecoreBranchDialog: IDialog
    {
        [NonSerialized]
        private DialogSequenceEngine _dialogSequenceExecutor;

        private ID _sitecoreItemId;

        public SitecoreBranchDialog(ID sitecoreItemId)
        {
            _sitecoreItemId = sitecoreItemId;
            _dialogSequenceExecutor = new DialogSequenceEngine(MessageReceivedAsync);
            _dialogSequenceExecutor.LoadActions(sitecoreItemId);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.PrivateConversationData.SetValue(
                "currentActionIndex", 0);
            context.PrivateConversationData.SetValue(
                "currentAction", _sitecoreItemId.ToString());

            var currentActionId = context.PrivateConversationData.GetValueOrDefault("currentAction", string.Empty);

            _dialogSequenceExecutor = new DialogSequenceEngine(MessageReceivedAsync);
            _dialogSequenceExecutor.LoadActions(ID.Parse(currentActionId));
            _dialogSequenceExecutor.ContinueExecution(this, context);
        }

        protected virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            Log.Info("SitecoreBranchDialog - MessageReceivedAsync", this);                        

            var currentActionId = context.PrivateConversationData.GetValueOrDefault("currentAction", string.Empty);

            _dialogSequenceExecutor = new DialogSequenceEngine(MessageReceivedAsync);
            _dialogSequenceExecutor.LoadActions(ID.Parse(currentActionId));
            _dialogSequenceExecutor.Resume(context, result).Wait();
        }
    }
}