using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SC90.Bot.Infrastructure.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using SC90.Bot.Infrastructure.Rules;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.Engine
{
    [Serializable]
    public class DialogSequenceEngine
    {
        [NonSerialized]
        private Item _dialogItem;
        [NonSerialized]
        private ChildList _dialogActions;
        [NonSerialized]
        private IDialog _dialog;

        private readonly ResumeAfter<IMessageActivity> _resumeAfter;
        private DialogStateContext _dialogStateContext;

        public DialogSequenceEngine(ResumeAfter<IMessageActivity> resumeAfter)
        {
            _resumeAfter = resumeAfter;
        }

        public void ContinueExecution(
            IDialog dialog,
            IDialogContext context)
        {
            _dialog = dialog;

            context.PrivateConversationData.SetValue("branchToGo", string.Empty);
            context.PrivateConversationData.SetValue("dialogToCall", string.Empty);

            RunActions(dialog, context);
        }

        private void RunActions(IDialog dialog, IDialogContext context)
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
                var dialogActionItem = _dialogActions[i];
                var dialogActionHandler = DialogActionFactory.CreateHandler(dialogActionItem);
                _dialogStateContext = new DialogStateContext();
                
                var dialogActionContext = new DialogActionContext(context, dialog)
                {
                    ActionState = _dialogStateContext
                };

                if (dialogActionHandler.IsPromptDialog)
                {
                    ((IPromptDialogAction)dialogActionHandler).Execute(dialogActionContext, Resume).Wait();
                }
                else
                {
                    ((IMessageDialogAction)dialogActionHandler).Execute(dialogActionContext).Wait();
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

                if (DoDialogBranchingIfRequired(dialogActionContext.Context, dialogActionItem))
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

            var actionItem = _dialogActions[currentActionIndex];

            var currentAction = (IPromptDialogAction)DialogActionFactory.CreateHandler(actionItem);
            currentAction.HandleDialogResult(context, _dialogStateContext, dialogResult).Wait();

            if (DoDialogBranchingIfRequired(context, actionItem))
            {
                return;
            }

            context.PrivateConversationData.SetValue("currentActionIndex", currentActionIndex + 1);            
            RunActions(_dialog, context);
        }

        private bool DoDialogBranchingIfRequired(IDialogContext context, Item actionItem)
        {
            var branchToGo = context.PrivateConversationData.GetValueOrDefault("branchToGo", string.Empty);
            var dialogToCall = context.PrivateConversationData.GetValueOrDefault("dialogToCall", string.Empty);

            if (!string.IsNullOrEmpty(branchToGo))
            {
                var branchItem = actionItem.Children[branchToGo];

                if (branchItem != null)
                {
                    context.Call(
                        child: new SitecoreBranchDialog(branchItem.ID),
                        resume: ResumeBranchExecution);
                }

                return true;
            }

            if (!string.IsNullOrEmpty(dialogToCall))
            {
                var dialogItem = Sitecore.Context.Database.GetItem(dialogToCall);

                if (dialogItem != null)
                {
                    context.Call(
                        child: new SitecoreBranchDialog(dialogItem.ID),
                        resume: ResumeDialogExecution);
                }

                return true;
            }

            return false;
        }

        private Task ResumeDialogExecution(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        private async Task ResumeBranchExecution<TResult>(IDialogContext context, IAwaitable<TResult> result)
        {
            var currentActionId = context.PrivateConversationData.GetValueOrDefault("currentAction", string.Empty);

            var branchItem = Sitecore.Context.Database.GetItem(ID.Parse(currentActionId));

            var parentExecutionRootItem = branchItem?.Parent?.Parent;
            var parentActionItem = branchItem?.Parent;

            if (parentActionItem == null || parentExecutionRootItem == null)
            {
                throw new InvalidOperationException("Something weird happened. Branch item expected.");
            }

            var parentActionItemIndex = parentExecutionRootItem.Children.IndexOf(parentActionItem);            

            context.PrivateConversationData.SetValue("currentActionIndex", parentActionItemIndex + 1);            
            context.PrivateConversationData.SetValue("currentAction", parentExecutionRootItem.ID.ToString());            

            LoadActions(parentExecutionRootItem.ID);
            
            RunActions(_dialog, context);
        }

        public void LoadActions(ID currentActionId)
        {
            _dialogItem = Sitecore.Context.Database.GetItem(currentActionId);
            _dialogActions = _dialogItem.Children;
        }
    }
}