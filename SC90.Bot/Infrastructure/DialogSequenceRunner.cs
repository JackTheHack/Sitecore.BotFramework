using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure
{
    [Serializable]
    public class DialogSequenceRunner
    {
        [NonSerialized]
        private Item _dialogItem;
        [NonSerialized]
        private ChildList _dialogActions;
        [NonSerialized]
        private IDialog _dialog;

        public async Task ContinueExecution(
            IDialog dialog,
            IDialogContext context)
        {
            _dialog = dialog;

            await RunActions(dialog, context);
        }

        private async Task RunActions(IDialog dialog, IDialogContext context)
        {
            var currentActionIndex = context.PrivateConversationData.GetValueOrDefault("currentActionIndex", 0);
            var currentActionId = context.PrivateConversationData.GetValueOrDefault("currentAction", string.Empty);

            if (currentActionIndex >= _dialogActions.Count)
            {
                context.Done(true);
                return;
            }

            for (int i = currentActionIndex; i < _dialogActions.Count; i++)
            {
                var dialogActionHandler = DialogActionFactory.CreateHandler(_dialogActions[i]);

                var dialogActionContext = new DialogActionContext(context, dialog);

                if (dialogActionHandler.IsPromptDialog)
                {
                    await ((IPromptDialogAction) dialogActionHandler).Execute(dialogActionContext, Resume);
                }
                else
                {
                    await ((IMessageDialogAction) dialogActionHandler).Execute(dialogActionContext);
                }

                if (dialogActionContext.ConversationCompleted)
                {
                    return;
                }

                if (dialogActionHandler.IsPromptDialog)
                {
                    context.PrivateConversationData.SetValue("currentActionIndex", i);
                    context.PrivateConversationData.SetValue("currentAction", currentActionId);
                    return;
                }
            }

            context.PrivateConversationData.SetValue("currentActionIndex", _dialogActions.Count);
            context.PrivateConversationData.SetValue("currentAction", currentActionId);
            context.Done(true);
        }

        public virtual async Task Resume(IDialogContext context, IAwaitable<object> result)
        {
            var dialogResult = await result;

            var currentActionIndex = context.PrivateConversationData.GetValueOrDefault("currentActionIndex", 0);
            var currentActionId = context.PrivateConversationData.GetValueOrDefault("currentAction", string.Empty);

            LoadActions(ID.Parse(currentActionId));

            var currentAction = (IPromptDialogAction)DialogActionFactory.CreateHandler(_dialogActions[currentActionIndex]);
            await currentAction.HandleDialogResult(context, dialogResult);            

            context.PrivateConversationData.SetValue("currentActionIndex", currentActionIndex + 1);

            await RunActions(_dialog, context);
        }

        public void LoadActions(ID currentActionId)
        {
            _dialogItem = Sitecore.Context.Database.GetItem(currentActionId);
            _dialogActions = _dialogItem.Children;
        }
    }
}