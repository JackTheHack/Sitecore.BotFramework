using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Services;

namespace SC90.Bot.Telegram
{
    public class ConfigureServices
    {
        public void Configure(IServiceCollection services)
        {
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddSingleton<ISitecoreBotService, SitecoreBotService>();
        }

    }
}
