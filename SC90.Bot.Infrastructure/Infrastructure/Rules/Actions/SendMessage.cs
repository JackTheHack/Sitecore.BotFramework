using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Sitecore.Rules.Actions;

namespace SC90.Bot.Infrastructure.Rules.Actions
{
    public class SendMessage<T> : RuleAction<T>
        where T : DialogRuleContext
    {
        public string Message { get; set; }

        public override async void Apply(T ruleContext)
        {
            Task.Run(async () => 
                ruleContext.DialogContext.PostAsync(Message)).Wait();
        }
    }
}