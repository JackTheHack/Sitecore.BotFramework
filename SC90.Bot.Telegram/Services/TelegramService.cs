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

            var telegramConfiguration = Factory.GetConfigNode("sitecoreBotFramework/telegram");

            if (telegramConfiguration == null)
            {
                throw new InvalidOperationException("Telegram configuration is missing");
            }

            var webHook = telegramConfiguration?.Attributes["webHookUrl"]?.Value;
            var botToken = botItem.BotToken;

            Configuration = new TelegramBotConfiguration
            {
                BotToken = botToken,
                WebHookEndpoint = webHook
            };
        }

        [Obsolete]
        private void GetConfigurationFromConfig()
        {
            var telegramConfiguration = Factory.GetConfigNode("sitecoreBotFramework/telegram");

            if (telegramConfiguration == null)
            {
                throw new InvalidOperationException("Telegram configuration is missing");
            }

            var botToken = telegramConfiguration?.Attributes?["botToken"]?.Value;
            var webHook = telegramConfiguration?.Attributes["webHookUrl"]?.Value;

            if (string.IsNullOrEmpty(botToken))
            {
                throw new InvalidOperationException("botToken is null");
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
