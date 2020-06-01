using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class CallDialog<T>: RuleAction<T>
        where T : DialogRuleContext
    {
        public string Dialog { get;set; }

        public override async void Apply(T ruleContext)
        {           
            ruleContext.GoToDialog = Dialog;
            ruleContext.Break = true;
        }

    }
}