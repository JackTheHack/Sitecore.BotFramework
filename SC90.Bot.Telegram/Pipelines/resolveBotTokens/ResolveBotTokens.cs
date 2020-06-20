using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SC90.Bot.Telegram.Models;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace SC90.Bot.Telegram.Pipelines.resolveBotTokens
{
    public class ResolveBotTokens
    {
        public void Process(ResolveTokenPipelineArgs resolveTokenArgs)
        {
            //var resolveTokenArgs = args as ResolveTokenPipelineArgs;

            if (resolveTokenArgs.Value.Contains("{{text}}"))
            {
                resolveTokenArgs.Value = resolveTokenArgs.Value.Replace("{{text}}",
                    resolveTokenArgs.BotContext.ChatUpdate.Message);
            }
        }
    }
}
