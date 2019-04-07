using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Dialogs;
using SC90.Bot.Infrastructure.Dialogs;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class GoToDialog<T>: RuleAction<T>
        where T : DialogRuleContext
    {
        public string Dialog { get;set; }

        public override async void Apply(T ruleContext)
        {           
            ruleContext.DialogContext.Reset();
            await Conversation.SendAsync(ruleContext.DialogContext.Activity.AsMessageActivity(),
                () => new RootDialog(Dialog));                        

            ruleContext.Break = true;
        }

    }
}