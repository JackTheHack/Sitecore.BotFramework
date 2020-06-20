using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;

namespace SC90.Bot.Telegram.Models
{
    public class ChatbotCommandContext
    {
        public ChatbotCommandContext()
        {
        }

        public _Bot Chatbot { get; set; }
        public _State CurrentState { get; set; }
        public _Command CommandContext { get; set; }
        public ChatUpdate ChatUpdate { get; set; }
        public string SessionKey { get; set; }
    }
}
