using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class SaveToVariable<T> : RuleAction<T>
        where T : DialogRuleContext
    {
        public string Variable { get; set; }

        public override async void Apply(T ruleContext)
        {
            ruleContext.DialogContext.PrivateConversationData.SetValue("VAR_"+Variable, ruleContext.Result);
        }
    }
}