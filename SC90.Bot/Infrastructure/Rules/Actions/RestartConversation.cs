using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Dialogs;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class RestartConversation<T>: RuleAction<T>
        where T : DialogRuleContext
    {
        public override async void Apply(T ruleContext)
        {           
            ruleContext.DialogContext.Reset();

            await Conversation.SendAsync(ruleContext.DialogContext.Activity.AsMessageActivity(),
                () => new RootDialog());            

            ruleContext.Break = true;
        }
    }
}