using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class SaveToVariable<T> : RuleAction<T>
        where T : DialogRuleContext
    {
        public string Message { get; set; }

        public override async void Apply(T ruleContext)
        {
            //TODO: Save dialog result to variable
        }
    }
}