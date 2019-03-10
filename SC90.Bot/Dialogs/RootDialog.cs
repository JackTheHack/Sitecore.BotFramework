using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace SC90.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog
    {
        [NonSerialized]
        private Item _botItem;
        [NonSerialized]
        private string _startDialogId;

        public RootDialog()
        {
        }

        public async Task StartAsync(IDialogContext context)
        {
            Log.Info("RootDialog - Start Async", this);

            _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
            _startDialogId = _botItem.Fields["StartDialog"].Value;

            context.Wait(MessageReceivedAsync);
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
            _startDialogId = _botItem.Fields["StartDialog"].Value;

            context.Call(
                new SitecoreDialog(ID.Parse(_startDialogId)),
                ResumeAfterDialogCompleted);

            return Task.CompletedTask;
        }

        protected async Task ResumeAfterDialogCompleted(IDialogContext context, IAwaitable<object> result)
        {
            Log.Info("RootDialog - Restart dialog.", this);

            _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
            _startDialogId = _botItem.Fields["StartDialog"].Value;

            //dialog complete run again root dialog
            context.PrivateConversationData.SetValue("currentActionIndex", 0);

            context.Wait(MessageReceivedAsync);
        }     
    }
}