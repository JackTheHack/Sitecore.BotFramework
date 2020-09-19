using Glass.Mapper.Sc;
using MongoDB.Bson;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Constants;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using System;
using System.Text.RegularExpressions;

namespace SC90.Bot.Telegram.Pipelines.resolveBotTokens
{
    public class ResolveBotTokens
    {
        public void Process(ResolveTokenPipelineArgs resolveTokenArgs)
        {
            var sessionProvider = ServiceLocator.ServiceProvider.GetService(typeof(ISessionProvider)) as ISessionProvider;
            var sitecoreService = new SitecoreService("web");

            if (resolveTokenArgs.Value.Contains("@{{text}}"))
            {
                resolveTokenArgs.Value = resolveTokenArgs.Value.Replace("@{{text}}",
                    resolveTokenArgs.BotContext.ChatUpdate.Message);
            }

            var sessionTokenMatches = Regex.Matches(resolveTokenArgs.Value, @"@\[\[(\w+)\]\]");

            var sessionDocument = sessionProvider.GetSessionDocument(resolveTokenArgs.BotContext.SessionId).Result;

            if (sessionDocument != null &&
                sessionTokenMatches != null && 
                sessionTokenMatches.Count > 0)
            {                

                    foreach (Match match in sessionTokenMatches)
                    {
                        var sessionValue = sessionDocument.GetValue(match.Groups[1].Value, new BsonString(string.Empty));
                        resolveTokenArgs.Value = resolveTokenArgs.Value.Replace(match.Value, sessionValue.ToString());                    
                    }
            }

            var contextItemTokenMatches = Regex.Matches(resolveTokenArgs.Value, @"@\(\((\w+)\)\)");

            if (contextItemTokenMatches != null && 
                contextItemTokenMatches.Count > 0)
            {
                if (sessionDocument.Contains(SessionConstants.Item))
                {
                    var contextItem = sessionDocument.GetValue(SessionConstants.Item);
                    if (contextItem != null && contextItem.AsGuid != Guid.Empty)
                    {
                        var item = sitecoreService.GetItem<Item>(contextItem.AsGuid);

                        if(item != null)
                        {
                            foreach (Match match in sessionTokenMatches)
                            {
                                string fieldName = match.Groups[1].Value;

                                if (!string.IsNullOrEmpty(fieldName) && 
                                    item.Fields[fieldName] != null)
                                {
                                    var itemFieldValue = item.Fields[fieldName].Value;
                                    resolveTokenArgs.Value = resolveTokenArgs.Value.Replace(match.Value, itemFieldValue);
                                }
                            }
                        }
                    }
                }                
            }

            //Z.Expressions.Eval.Compile()
        }
    }
}
