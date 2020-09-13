using SC90.Bot.Telegram.Models;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Telegram.RuleActions
{
    public class GoToDecisionBranchRuleAction<T> : RuleAction<T>
        where T : ChatbotRuleContext
    {
        public string Branch { get;set; }

        public override void Apply(T ruleContext)
        {
            ruleContext.SelectedDecisionBranch = Branch;
        }
    }
}
