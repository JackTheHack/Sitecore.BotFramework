using System;
using Microsoft.Extensions.Options;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.Configuration;
using Telegram.Bot;

namespace SC90.Bot.Telegram.Services
{
    public class TelegramService : ITelegramService
    {
        public TelegramBotConfiguration Configuration { get; set; }

        public TelegramService()
        {
            if (Configuration == null)
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
            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(Configuration.BotToken);
        }

        public TelegramBotClient Client { get; }
    }
}
