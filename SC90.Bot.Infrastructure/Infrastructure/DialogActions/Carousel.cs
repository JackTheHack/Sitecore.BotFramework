using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using SC90.Bot.Helpers;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class Carousel : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _dialogAction;

        public Carousel(Item dialogAction)
        {
            _dialogAction = dialogAction;
        }

        public bool IsPromptDialog => false;

        public Task Execute(DialogActionContext context)
        {
            var message = context.Context.MakeMessage();
            message.Attachments = new List<Attachment>();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            var childCards = _dialogAction.Children;

            foreach (Item childCard in childCards)
            {
                var card = DialogueHelper.CreateCard(childCard);
                message.Attachments.Add(card.ToAttachment());
            }

            context.Context.PostAsync(message);

            return Task.CompletedTask;
        }
    }
}