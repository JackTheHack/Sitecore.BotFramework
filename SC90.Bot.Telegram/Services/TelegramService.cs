using Microsoft.Extensions.Options;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.Configuration;
using Telegram.Bot;

namespace SC90.Bot.Telegram.Services
{
    public class TelegramService : ITelegramService
    {
        static TelegramBotConfiguration _configuration;

        public TelegramService()
        {
            if (_configuration == null)
            {
                var telegramConfiguration = Factory.GetConfigNode("sitecoreBotFramework/telegram");
                _configuration = new TelegramBotConfiguration
                {
                    BotToken = telegramConfiguration["botToken"]?.Value
                };
            }
            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(_configuration.BotToken);
        }

        public TelegramBotClient Client { get; }
    }
}
