using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using System;

namespace SC90.Bot.Telegram.Services
{
    public class RuleEngineService : IRuleEngineService
    {
        public bool RunRules<T>(Item item, string fieldName, T ruleContext)
            where T : ChatbotRuleContext
        {
            var rulesList = RuleFactory.GetRules<T>(new[] {item}, fieldName).Rules;
            ruleContext.Result = false;
            foreach (Rule<T> rule in rulesList)
            {
                if (rule.Condition != null)
                {
                    var stack = new RuleStack();

                    try
                    {
                        rule.Condition.Evaluate(ruleContext, stack);
                    }catch(Exception e)
                    {
                        ruleContext.Result = false;
                        ruleContext.HasError = true;
                        Log.Error("Failed to execute rule - " + e, this);
                    }
 
                    if (ruleContext.IsAborted)
                    {
                        continue;
                    }

                    if (stack.Count != 0 && (bool)stack.Pop())
                    {
                        ruleContext.Result = true;
                        rule.Execute(ruleContext);
                    }
                }
                else
                {
                    ruleContext.Result = true;
                    rule.Execute(ruleContext);
                }
            }

            return ruleContext.Result;
        }
    }
}
