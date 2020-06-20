using System.Web.Http;
using System.Web.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Services;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Telegram.Bot;

namespace SC90.Bot.Telegram
{
    public class ConfigureServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection services)
        {
            RegisterRoute(RouteTable.Routes);

            services.AddTransient<ITelegramService, TelegramService>();
            services.AddTransient<ISitecoreBotService, SitecoreBotService>();
            services.AddTransient<ISessionProvider, MongoSessionProvider>();
            services.AddTransient<IRuleEngineService, RuleEngineService>();
            services.AddTransient<ICommandService, CommandService>();
            services.AddTransient<IDialogActionFactory, DialogActionFactory>();
        }

        private void RegisterRoute(RouteCollection routes)
        {
            RouteTable.Routes.MapHttpRoute("PingTelegramApiController", 
                "sitecore/api/TelegramApi/{action}", /* do not include a forward slash in front of the route */
                new {controller = "TelegramApi", action  = "Ping"} /* controller name should not have the "Controller" suffix */
            );

            routes.MapHttpRoute("TelegramApiController", "sitecore/api/TelegramApi/{botId}/{action}", new
            {
                controller = "TelegramApi",
                action = "Post", 
                id = RouteParameter.Optional
            });
        }
    }
}
