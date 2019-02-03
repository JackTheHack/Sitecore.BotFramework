using Microsoft.Bot.Builder.Dialogs;
using Sitecore.Rules;

namespace SC90.Bot.Infrastructure.Rules
{
    public class DialogRuleContext : RuleContext
    {
        public IDialog Dialog { get; set; }
        public IDialogAction Action { get; set; }
        public IDialogContext DialogContext { get; set; }
        public string GoToDecisionBranch { get; set; }
        public string GoToDialog { get; set; }
        public object Result { get; set; }
    }
}