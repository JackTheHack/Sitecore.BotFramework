using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SC90.Bot.Helpers;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace SC90.Bot.Infrastructure.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog
    {
        [NonSerialized]
        private Item _botItem;
        [NonSerialized]
        private string _startDialogId;

        private string _options;

        private Item BotItem
        {
            get
            {
                if (_botItem != null)
                {
                    return _botItem;
                }

                _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
                return _botItem;
            }
        }

        public RootDialog()
        {
        }

        public RootDialog(string startDialog, Activity startActivity = null)
        {
            _startDialogId = startDialog;
        }

        public async Task StartAsync(IDialogContext context)
        {
            Log.Info("RootDialog - Start Async", this);

            if (string.IsNullOrEmpty(_startDialogId))
            {
                _startDialogId = BotItem.Fields["StartDialog"].Value;
            }

            context.Wait(MessageReceivedAsync);
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            _startDialogId = BotItem.Fields["StartDialog"].Value;
            
            context.Call(
                new SitecoreDialog(ID.Parse(_startDialogId)),
                ResumeAfterDialogCompleted);

            return Task.CompletedTask;
        }

        protected async Task ResumeAfterDialogCompleted(IDialogContext context, IAwaitable<object> result)
        {
            Log.Info("RootDialog - Restart dialog.", this);
            
            _options = BotItem.Fields["Options"].Value;
            _startDialogId = BotItem.Fields["StartDialog"].Value;

            //dialog complete run again root dialog
            context.PrivateConversationData.SetValue("currentActionIndex", 0);

            if (!string.IsNullOrEmpty(_options))
            {
                var reply = context.MakeMessage();
                reply.Attachments = new List<Attachment>();
                var card = DialogueHelper.CreateOptionsCard(BotItem);
                reply.Attachments.Add(card.ToAttachment());
                await context.PostAsync(reply);
            }

            context.Wait(MessageReceivedAsync);
        }     

        
    }
}