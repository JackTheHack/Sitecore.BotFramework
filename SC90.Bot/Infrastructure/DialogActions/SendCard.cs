using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SC90.Bot.Infrastructure.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class SendMessage : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _dialogAction;
        private readonly string _message;
        private readonly string _url;
        private readonly string _image;
        private readonly string _title;
        private readonly string _subtitle;

        public SendMessage(Item dialogAction)
        {
            _dialogAction = dialogAction;
            if (!string.IsNullOrEmpty(dialogAction["Url"]))
            {
                var linkField = (LinkField) dialogAction.Fields["Url"];
                _url = linkField.Url;
            }
            _title = _dialogAction["Title"];
            _subtitle = dialogAction["Subtitle"];
            if (!string.IsNullOrEmpty(dialogAction["Image"]))
            {
                var imageField = (ImageField) dialogAction.Fields["Image"];
                _image = MediaManager.GetMediaUrl(imageField.MediaItem, new MediaUrlOptions()
                {
                    AlwaysIncludeServerUrl = true
                });
            }
            _message = _dialogAction["Text"];
        }

        public bool IsPromptDialog => false;

        public Task Execute(DialogActionContext context)
        {
            if (string.IsNullOrEmpty(_url) &&
                string.IsNullOrEmpty(_title) &&
                string.IsNullOrEmpty(_image))
            {
                Task.Run(() => context.Context.PostAsync(_message)).Wait();
            }
            else
            {
                var cardAction = 
                    !string.IsNullOrEmpty(_url) ?
                    new CardAction(ActionTypes.OpenUrl, _title, value: _url) : null;
                
                var cardImage = 
                    !string.IsNullOrEmpty(_image) ?
                    new CardImage(_image, _title, cardAction) : null;

                var heroCard = new HeroCard(_title, _subtitle, _message, 
                    cardImage != null?
                    new List<CardImage>() {cardImage} : null, tap: cardAction);

                var message = context.Context.MakeMessage();
                message.Attachments.Add(
                    heroCard.ToAttachment());

                //await context.Context.PostAsync(message).Wait();

                Task.Run( () =>
                    context.Context.PostAsync(message)).Wait();

                Log.Audit($"SendCard for {_dialogAction.Name} executed.", this);
            }

            return Task.CompletedTask;
        }
    }
}