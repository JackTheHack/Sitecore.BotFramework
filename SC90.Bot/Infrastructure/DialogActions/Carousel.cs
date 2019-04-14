﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Infrastructure.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class Carousel : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _dialogAction;
        private readonly string _message;

        public Carousel(Item dialogAction)
        {
            _dialogAction = dialogAction;
            _message = _dialogAction["Text"];
        }

        public bool IsPromptDialog => false;

        public Task Execute(DialogActionContext context)
        {
            Task.Run(async () => context.Context.PostAsync(_message)).Wait();
            return Task.CompletedTask;
        }
    }
}