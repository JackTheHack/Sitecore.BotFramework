using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SC90.Bot.Infrastructure;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace SC90.Bot.Dialogs
{
    [Serializable]
    public class SitecoreDialog : IDialog
    {
        [NonSerialized]
        private Item _dialogItem;
        [NonSerialized]
        private ChildList _dialogActions;

        private readonly ID _sitecoreItemId;

        public SitecoreDialog(ID sitecoreItemId)
        {
            _sitecoreItemId = sitecoreItemId;
            LoadActions();
        }

        public void LoadActions()
        {
            _dialogItem = Sitecore.Context.Database.GetItem(_sitecoreItemId);
            _dialogActions = _dialogItem.Children;
        }

        public async Task StartAsync(IDialogContext context)
        {
            Log.Info("SitecoreDialog - Start Async", this);

            context.PrivateConversationData.SetValue(
                "currentActionIndex", 0);
            context.Wait(MessageReceivedAsync);                        
        }

        protected virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            Log.Info("SitecoreDialog - MessageReceivedAsync", this);
            
            LoadActions();

            var currentActionIndex = context.PrivateConversationData.GetValueOrDefault("currentActionIndex", 0);

            if (currentActionIndex >= _dialogActions.Count)
            {
                context.Done(true);   
                return;
            }

            for (int i = currentActionIndex; i < _dialogActions.Count; i++)
            {
                var dialogActionHandler = DialogActionFactory.CreateHandler(_dialogActions[i]);
                
                await dialogActionHandler.Execute(context, this);

                if (dialogActionHandler.IsPromptDialog)
                {
                    context.PrivateConversationData.SetValue("currentActionIndex", i+1);                    
                    context.Wait(MessageReceivedAsync);
                    return;
                }
            }

            context.PrivateConversationData.SetValue("currentActionIndex", _dialogActions.Count);
            //context.Wait(MessageReceivedAsync);            
            context.Done(true);
        }
    }
}