using System;
using SC90.Bot.Infrastructure.Extensions;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace SC90.Bot.Infrastructure.Rules
{
    [Serializable]
    public class DialogRuleEngine
    {
        public void RunRules<T>(Item item, string fieldName, T ruleContext)
            where T : DialogRuleContext
        {
            foreach (Rule<T> rule in RuleFactory.GetRules<T>(new[] { item }, fieldName).Rules)
            {
                if (ruleContext.Break)
                {
                    break;
                }

                if (rule.Condition != null)
                {
                    var stack = new RuleStack();
                    rule.Condition.Evaluate(ruleContext, stack);
 
                    if (ruleContext.IsAborted)
                    {
                        continue;
                    }

                    if ((stack.Count != 0) && ((bool)stack.Pop()))
                    {
                        rule.Execute(ruleContext);
                    }
                }
                else
                    rule.Execute(ruleContext);
            }

                ruleContext.DialogContext.PrivateConversationData
                    .SetValueOrRemoveIfNull("branchToGo",
                    ruleContext.GoToDecisionBranch ?? string.Empty);
         
                ruleContext.DialogContext.PrivateConversationData
                    .SetValueOrRemoveIfNull("dialogToCall", 
                        ruleContext.GoToDialog ?? string.Empty);
        }
    }
}