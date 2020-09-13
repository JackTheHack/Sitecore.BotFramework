using System;
using Glass.Mapper.Sc;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.Configuration;
using Telegram.Bot;

namespace SC90.Bot.Telegram.Services
{
    public class TelegramService : ITelegramService
    {
        private SitecoreService _sitecoreService;

        public TelegramBotConfiguration Configuration { get; set; }

        public TelegramService(IBotRequestContext botContext)
        {
            _sitecoreService = new SitecoreService("web");

            if (Configuration == null)
            {
                LoadBotConfiguration(botContext.BotId);
            }

            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(Configuration.BotToken);
        }

        private void LoadBotConfiguration(Guid botId)
        {
            var botItem = _sitecoreService.GetItem<_Bot>(botId);

            if(botItem == null)
            {
                throw new ArgumentException("Cannot load configuration for bot " + botId.ToString());
            }

            var chatbotSettings = _sitecoreService.GetItem<_Settings>("/sitecore/system/Settings/Feature/SitecoreBotFrameworkV2/Settings");
            var webHook = chatbotSettings?.HostUrl;

            var telegramConfiguration = Factory.GetConfigNode("sitecoreBotFramework/telegram");

            if (string.IsNullOrEmpty(webHook))
            {
                webHook = telegramConfiguration?.Attributes["webHookUrl"]?.Value;
            }

            var botToken = botItem.BotToken;

            if(string.IsNullOrEmpty(botToken))
            {
                botToken = telegramConfiguration?.Attributes["botToken"]?.Value;
            }

            Configuration = new TelegramBotConfiguration
            {
                BotToken = botToken,
                WebHookEndpoint = webHook
            };
        }

        public TelegramBotClient Client { get; }
    }
}
