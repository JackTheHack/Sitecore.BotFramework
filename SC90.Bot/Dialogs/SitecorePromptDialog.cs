﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace SC90.Bot.Dialogs
{
    [Serializable]
    public class SitecorePromptDialog : IDialog<object>
    {
        private int _attempts;
        private readonly string _promptMessage;
        private readonly string _validationMessage;
        private readonly string _image;
        private readonly string _options;
        private readonly string _title;
        private readonly string _validationType;
        private string[] _optionsStrings;

        public SitecorePromptDialog(Item promptDialogItem)
        {
            _promptMessage = promptDialogItem["Message"];
            _validationMessage = promptDialogItem["ValidationMessage"];

            if (string.IsNullOrEmpty(_validationMessage))
            {
                _validationMessage = "Sorry didn't catched that. Lets try again.";
            }

            _validationType = promptDialogItem["Type"];
            _options = promptDialogItem["ChoiceOptions"];
            _title = promptDialogItem["Title"];

            if (!string.IsNullOrEmpty(promptDialogItem["Image"]))
            {
                var imageField = (ImageField) promptDialogItem.Fields["Image"];
                _image = MediaManager.GetMediaUrl(imageField.MediaItem, new MediaUrlOptions()
                {
                    AlwaysIncludeServerUrl = true
                });
            }
        }

        public Task StartAsync(IDialogContext context)
        {
            if (string.IsNullOrEmpty(_options) &&
                string.IsNullOrEmpty(_image) &&
                string.IsNullOrEmpty(_title))
            {
                Task.Run(async ()=>context.PostAsync(_promptMessage));
            }
            else
            {
                var message = context.MakeMessage();
                var cardImage = 
                    !string.IsNullOrEmpty(_image) ?
                    new CardImage(_image) : null;

                var imageList = cardImage != null ? new List<CardImage>() {cardImage} : null;

                var heroImage = new HeroCard(
                    _title, 
                    text:_promptMessage,
                    images: imageList);      

                message.Attachments.Add(heroImage.ToAttachment());
                
                if (!string.IsNullOrEmpty(_options))
                {
                    var basicCard = new HeroCard();                    

                    _optionsStrings = _options.Split(new[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);                                       

                    basicCard.Buttons = new List<CardAction>();

                    foreach (var suggestedString in _optionsStrings)
                    {
                        var actionText = suggestedString.Trim(new char[] {' ', '\r'});
                        basicCard.Buttons.Add(new CardAction(ActionTypes.MessageBack, 
                            actionText, 
                            value:actionText, 
                            text:actionText));
                    }

                    message.Attachments.Add(basicCard.ToAttachment());
                }

                

                Task.Run(async () => context.PostAsync(message).Wait());
            }

            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            /* If the message returned is a valid name, return it to the calling dialog. */
            if (ValidateInput(message))
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.Done(message.Text);
            }
            /* Else, try again by re-prompting the user. */
            else
            {
                --_attempts;

                if (_attempts > 0)
                {
                    await context.PostAsync(_validationMessage);

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                        parent/calling dialog. */
                    context.Fail(new TooManyAttemptsException("Validation failed."));
                }
            }
        }

        protected virtual bool ValidateInput(IMessageActivity message)
        {
            if(string.IsNullOrEmpty(message.Text))
            {
                return false;
            }

            switch(_validationType)

            {
                case "Number":
                    return decimal.TryParse(message.Text.Trim(), out var result);
                case "Choice":
                    return _optionsStrings != null && _optionsStrings.Contains(message.Text.Trim());
                case "Confirm":
                    return bool.TryParse(message.Text.Trim(), out var result2) || 
                           message.Text.ToLower() == "yes" ||
                           message.Text.ToLower() == "no";
                case "DateTime":
                    return DateTime.TryParse(message.Text.Trim(), out var result3);
            }

            return true;
        }
    }
}