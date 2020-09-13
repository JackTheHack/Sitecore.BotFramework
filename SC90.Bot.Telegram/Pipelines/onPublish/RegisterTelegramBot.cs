using Glass.Mapper.Sc;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Events;
using System;
using System.Linq;

namespace SC90.Bot.Telegram.Pipelines.onPublish
{
    public class RegisterTelegramBot
    {
        public void OnItemSaved(object sender, EventArgs args) {
            try
            {                
                var item = Event.ExtractParameter<Item>(args, 0);

                if (item != null && item.Template.BaseTemplates.Any(i => i.ID == I_BotConstants.TemplateId))
                {
                    Sitecore.Diagnostics.Log.Info($"{item.Name} - Registering bot endpoints...", this);

                    var sitecoreService = new SitecoreService("web");

                    IBotRequestContext botRequestContext = ServiceLocator.ServiceProvider.GetService<IBotRequestContext>();

                    var bot = sitecoreService.GetItem<_Bot>(item);

                    if (bot != null && !string.IsNullOrEmpty(bot.BotToken))
                    {
                        Sitecore.Diagnostics.Log.Info($"{bot.Name} - Found Telegram token - registering bot with Telegram...", this);

                        botRequestContext.SetBotContext(bot.Id, true);

                        ISitecoreBotService botService = ServiceLocator.ServiceProvider.GetService<ISitecoreBotService>();

                        botService.Register("telegram", bot.Route, string.Empty);
                    }

                    Sitecore.Diagnostics.Log.Info($"{item.Name} - Registering bot endpoints - done...", this);
                }
            }catch(Exception e)
            {
                Sitecore.Diagnostics.Log.Warn("Failed to register bot - " + e.ToString(), this);
            }
        }
 
}
}
