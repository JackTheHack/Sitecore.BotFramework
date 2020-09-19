using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;

namespace SC90.Bot.Telegram.Models
{
    public class ChatbotPipelineContext
    {
        public ChatbotPipelineContext()
        {
        }

        public ChatbotPipelineContext(ChatbotActionContext _context)
        {
            CurrentState = _context.CurrentState;
            Chatbot = _context.Chatbot;
            CommandContext = _context.CommandContext;
            ChatUpdate = _context.ChatUpdate;
            SessionId = _context.SessionKey;
        }

        public ChatbotPipelineContext(ChatbotRuleContext ruleContext)
        {
            CurrentState = ruleContext.CurrentState;
            Chatbot = ruleContext.Chatbot;
            CommandContext = ruleContext.CommandContext;
            ChatUpdate = ruleContext.ChatUpdate;
            SessionId = ruleContext.SessionId;
        }

        public _Bot Chatbot { get; set; }
        public _State CurrentState { get; set; }
        public _Command CommandContext { get; set; }
        public ChatUpdate ChatUpdate { get; set; }
        public string SessionId { get; internal set; }
    }
}
