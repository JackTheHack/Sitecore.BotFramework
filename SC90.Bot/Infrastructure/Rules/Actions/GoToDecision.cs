using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class GoToDecision<T> : RuleAction<T>
        where T : DialogRuleContext
    {
        public string Branch { get;set; }

        public override async void Apply(T ruleContext)
        {
            await ruleContext.DialogContext.PostAsync("TODO: Run decision branch " + Branch);
        }
    }
}