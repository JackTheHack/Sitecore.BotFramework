using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;

namespace SC90.Bot.Telegram.Models
{
    public class ChatbotActionContext
    {
        public ChatbotActionContext()
        {
        }

        public ChatbotActionContext(ChatbotCommandContext context, _DialogAction action)
        {
            ChatUpdate = context.ChatUpdate;
            CurrentState = context.CurrentState;
            CommandContext = context.CommandContext;
            Chatbot = context.Chatbot;
            ActionContext = action;
            SessionKey = context.SessionKey;
            SchedulingData = context.SchedulingData;
        }

        public ChatbotActionContext(ChatbotDialogueContext context, _DialogAction command, _Dialogue dialogue)
        {
            Chatbot = context.Chatbot;
            ChatUpdate = context.ChatUpdate;
            ActionContext = command;
            CurrentState = context.CurrentState;
            SessionKey = context.SessionKey;
            DialogueContext = dialogue;
            SchedulingData = context.SchedulingData;
        }

        public _Bot Chatbot { get; set; }
        public _State CurrentState { get; set; }
        public _Command CommandContext { get; set; }
        public _DialogAction ActionContext { get; set; }
        public ChatUpdate ChatUpdate { get; set; }
        public string SessionKey { get; set; }
        public _Dialogue DialogueContext { get; set; }
        public SchedulingJobData SchedulingData { get; set; }
    }
}
