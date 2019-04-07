using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Infrastructure.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using SC90.Bot.Infrastructure.Rules;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class Condition : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _item;

        private readonly DialogRuleEngine _ruleEngine;

        public Condition(Item dialogAction)
        {
            _item = dialogAction;
            _ruleEngine = new DialogRuleEngine();
        }

        public bool IsPromptDialog => false;

        public Task Execute(DialogActionContext context)
        {
            var ruleContext = new DialogRuleContext()
            {
                DialogContext = context.Context,
                Dialog = context.Dialog,
                Action = this,
                Result = null,
                Item = _item
            };

            _ruleEngine.RunRules(_item, "Rule", ruleContext);           

            return Task.CompletedTask;
        }
    }
}