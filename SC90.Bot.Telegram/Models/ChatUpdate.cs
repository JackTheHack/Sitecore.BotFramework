using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC90.Bot.Telegram.Models
{
    public class ChatUpdate
    {
        public string Source { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
    }
}
