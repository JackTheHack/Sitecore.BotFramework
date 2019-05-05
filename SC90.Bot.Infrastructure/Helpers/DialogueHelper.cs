using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace SC90.Bot.Helpers
{
    public static class DialogueHelper
    {
        public static HeroCard CreateCard(Item dialogAction)
        {
            string url = string.Empty;
            if (!string.IsNullOrEmpty(dialogAction["Url"]))
            {
                var linkField = (LinkField) dialogAction.Fields["Url"];
                url = linkField.Url;
            }
            var title = dialogAction["Title"];
            var subtitle = dialogAction["Subtitle"];
            var image = string.Empty;
            if (!string.IsNullOrEmpty(dialogAction["Image"]))
            {
                var imageField = (ImageField) dialogAction.Fields["Image"];
                image = MediaManager.GetMediaUrl(imageField.MediaItem, new MediaUrlOptions()
                {
                    AlwaysIncludeServerUrl = true
                });
            }
            var message = dialogAction["Text"];

            return CreateCard(url, title, image, subtitle, message);
        }

        public static HeroCard CreateCard(string url, string title, string image, string subtitle, string message)
        {
            var cardAction =
                !string.IsNullOrEmpty(url) ? new CardAction(ActionTypes.OpenUrl, title, value: url) : null;

            var cardImage =
                !string.IsNullOrEmpty(image) ? new CardImage(image, title, cardAction) : null;

            var heroCard = new HeroCard(title, subtitle, message,
                cardImage != null ? new List<CardImage>() {cardImage} : null, tap: cardAction);
            return heroCard;
        }

        public static HeroCard CreateOptionsCard(Item botItem)
        {   
            var options = botItem.Fields["Options"].Value;
            var optionsTitle = botItem.Fields["OptionsTitle"].Value;

            var basicCard = new HeroCard();

            if (!string.IsNullOrEmpty(options))
            {
                var optionsStrings =
                    options.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(i => i.Trim(' ', '\r', '\t'))
                        .ToArray();

                basicCard.Buttons = new List<CardAction>();

                foreach (var suggestedString in optionsStrings)
                {
                    var actionText = suggestedString;
                    basicCard.Buttons.Add(new CardAction(ActionTypes.MessageBack,
                        actionText,
                        value: actionText,
                        text: actionText));
                }
            }

            if (!string.IsNullOrEmpty(optionsTitle))
            {
                basicCard.Title = optionsTitle;
            }

            return basicCard;
        }
    }
}