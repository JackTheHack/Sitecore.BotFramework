using MongoDB.Bson;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.DependencyInjection;
using System.Text.RegularExpressions;

namespace SC90.Bot.Telegram.Pipelines.resolveBotTokens
{
    public class ResolveBotTokens
    {
        public void Process(ResolveTokenPipelineArgs resolveTokenArgs)
        {
            var sessionProvider = ServiceLocator.ServiceProvider.GetService(typeof(ISessionProvider)) as ISessionProvider;

            if (resolveTokenArgs.Value.Contains("@{{text}}"))
            {
                resolveTokenArgs.Value = resolveTokenArgs.Value.Replace("@{{text}}",
                    resolveTokenArgs.BotContext.ChatUpdate.Message);
            }

            var sessionTokenMatches = Regex.Matches(resolveTokenArgs.Value, @"@\[\[(\w+)\]\]");
           
            if(sessionTokenMatches != null && sessionTokenMatches.Count > 0)
            {
                var sessionDocument = sessionProvider.GetSessionDocument(resolveTokenArgs.BotContext.SessionId).Result;

                if (sessionDocument != null)
                {
                    foreach (Match match in sessionTokenMatches)
                    {
                        var sessionValue = sessionDocument.GetValue(match.Groups[1].Value, new BsonString(string.Empty));
                        resolveTokenArgs.Value = resolveTokenArgs.Value.Replace(match.Value, sessionValue.ToString());                    
                    }
                }
            }
            
            //Z.Expressions.Eval.Compile()
        }
    }
}
