using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Dialogs;
using SC90.Bot.Infrastructure.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using SC90.Bot.Infrastructure.Rules;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure.DialogActions
{
    [Serializable]
    public class Prompt : IPromptDialogAction
    {
        [NonSerialized]
        private readonly Item _item;
        
        private IDialog _dialog;
        private readonly DialogRuleEngine _ruleEngine;
        public bool IsPromptDialog => true;

        public Prompt(Item item)
        {
            _item = item;
            _ruleEngine = new DialogRuleEngine();
        }

        public Task Execute(DialogActionContext context, ResumeAfter<object> resumeAction)
        {
            _dialog = context.Dialog;
            context.Context.Call(new SitecorePromptDialog(_item), resumeAction);                        
            
            return Task.CompletedTask;
        }

        public Task HandleDialogResult(IDialogContext context, DialogStateContext dialogStateContext,
            object dialogResult)
        {
            var ruleContext = new DialogRuleContext()
            {
                DialogContext = context,
                Dialog = _dialog,
                Action = this,
                Result = dialogResult,
                Item = _item
            };

            _ruleEngine.RunRules(_item, "Action", ruleContext);           

            return Task.CompletedTask;
        }
    }
}