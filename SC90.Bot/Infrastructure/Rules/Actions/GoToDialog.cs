using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class GoToDialog<T>: RuleAction<T>
        where T : DialogRuleContext
    {
        public string Dialog { get;set; }

        public override async void Apply(T ruleContext)
        {           
            ruleContext.GoToDialog = Dialog;
            //await ruleContext.DialogContext.PostAsync("TODO: Run decision branch " + Dialog);
        }

    }
}