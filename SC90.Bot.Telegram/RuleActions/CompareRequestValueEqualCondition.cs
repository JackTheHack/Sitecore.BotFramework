using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.Telegram.Models;
using Sitecore.Pipelines;
using Sitecore.Rules.Conditions;

namespace SC90.Bot.Telegram.RuleActions
{
    public class CompareRequestValueEqualCondition<T> : StringOperatorCondition<T>
        where T : ChatbotRuleContext
    {
        public string Value { get; set; }

        protected override bool Execute(T ruleContext)
        {
            var pipelineContext = new ChatbotPipelineContext()
            {
                CurrentState = ruleContext.CurrentState,
                Chatbot                = ruleContext.Chatbot,
                CommandContext = ruleContext.CommandContext,
                ChatUpdate = ruleContext.ChatUpdate
            };

            var value1 = ruleContext.ChatUpdate.Message;

            var tokenArgs = new ResolveTokenPipelineArgs() {Value = Value, BotContext = pipelineContext};
            CorePipeline.Run("resolveBotTokens", tokenArgs);
            var value2 = tokenArgs.Value;

            return Compare(value1, value2);
        }
    }
}
