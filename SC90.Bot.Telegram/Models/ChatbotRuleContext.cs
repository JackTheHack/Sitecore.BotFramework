using System;
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

        public ChatbotRuleContext(ChatbotActionContext _context)
        {
            Result = false;
            Chatbot = _context.Chatbot;
            ChatUpdate = _context.ChatUpdate;
            CommandContext = _context.CommandContext;
            CurrentState = _context.CurrentState;
            SessionId = _context.SessionKey;
        }

        public _Bot Chatbot { get; set; }
        public _State CurrentState { get; set; }
        public _Command CommandContext { get; set; }
        public ChatUpdate ChatUpdate { get; set; }
        public string SelectedDecisionBranch { get; set; }
        public bool Result { get; set; }
        public bool HasError { get; set; }
        public SchedulingJobData SchedulingData { get; set; }
        public string SessionId { get; internal set; }
    }
}
