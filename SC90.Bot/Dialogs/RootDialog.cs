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

            //context.Wait(this.MessageReceivedAsync); 

            _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
            _startDialogId = _botItem.Fields["StartDialog"].Value;

            //var message = await item;
            
            //await context.Forward(
            //    new SitecoreDialog(ID.Parse(_startDialogId)),
            //    ResumeAfterDialogCompleted, 
            //    message, 
            //    CancellationToken.None);

            context.Call(
                new SitecoreDialog(ID.Parse(_startDialogId)),
                ResumeAfterDialogCompleted);
        }

        protected virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            Log.Info("RootDialog - MessageReceivedAsync", this);

            _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
            _startDialogId = _botItem.Fields["StartDialog"].Value;

            var message = await item;
            
            //await context.Forward(
            //    new SitecoreDialog(ID.Parse(_startDialogId)),
            //    ResumeAfterDialogCompleted, 
            //    message, 
            //    CancellationToken.None);

            context.Call(
                new SitecoreDialog(ID.Parse(_startDialogId)),
                ResumeAfterDialogCompleted);

        }

        protected async Task ResumeAfterDialogCompleted(IDialogContext context, IAwaitable<object> result)
        {
            Log.Info("RootDialog - ResumeAfterDialogCompleted", this);

            _botItem = Sitecore.Context.Database.GetItem(ID.Parse("{E5F3FCCE-22DA-40AF-85F6-9F7D40E45EEF}"));
            _startDialogId = _botItem.Fields["StartDialog"].Value;

            //dialog complete run again root dialog
            context.PrivateConversationData.SetValue("currentActionIndex", 0);
            
            //context.Wait(this.MessageReceivedAsync);

            context.Call(
                new SitecoreDialog(ID.Parse(_startDialogId)),
                ResumeAfterDialogCompleted);
        }     
    }
}