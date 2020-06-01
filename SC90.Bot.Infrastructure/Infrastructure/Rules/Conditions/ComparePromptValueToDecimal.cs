using System;
using System.Linq.Expressions;
using Sitecore.Rules.Conditions;

namespace SC90.Bot.Infrastructure.Rules.Conditions
{
    public class ComparePromptValueToDecimal<T>  : StringOperatorCondition<T>
        where T : DialogRuleContext
    {
        public string Value { get; set; }

        protected override bool Execute(T ruleContext)
        {
            var promptValue = ruleContext.Result.ToString();

            return Compare(Value, promptValue);
        }
    }
}