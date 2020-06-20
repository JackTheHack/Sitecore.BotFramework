using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines;

namespace SC90.Bot.Telegram.Models
{
    public class ResolveTokenPipelineArgs : PipelineArgs
    {
        public string Value { get; set; }
        public ChatbotPipelineContext BotContext { get; set; }
    }
}
