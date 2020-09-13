using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;

namespace SC90.Bot.Telegram.Models
{
    public class ChatbotActionContext
    {
        public ChatbotActionContext()
        {
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
