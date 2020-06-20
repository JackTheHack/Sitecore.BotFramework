using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using Sitecore.Rules;

namespace SC90.Bot.Telegram.Models
{
    public class ChatbotRuleContext : RuleContext
    {
        public ChatbotRuleContext()
        {
            Result = false;
        }

        public _Bot Chatbot { get; set; }
        public _State CurrentState { get; set; }
        public _Command CommandContext { get; set; }
        public ChatUpdate ChatUpdate { get; set; }
        public bool Result { get; set; }
    }
}
