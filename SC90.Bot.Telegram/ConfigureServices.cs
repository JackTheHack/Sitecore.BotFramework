using System.Web.Http;
using System.Web.Routing;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Mvc;
using SC90.Bot.Telegram.Services;
using Sitecore.DependencyInjection;

namespace SC90.Bot.Telegram
{
    public class ConfigureServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection services)
        {
            RegisterRoute(RouteTable.Routes);

            var schedulerService = new SchedulerService();
            schedulerService.Initialize().Wait();

            services.AddScoped<ISessionProvider, MongoSessionProvider>();
            services.AddScoped<IBotRequestContext, BotRequestContext>();
            services.AddSingleton<ISchedulerService, SchedulerService>(x => schedulerService);
            services.AddTransient<ITelegramService, TelegramService>();
            services.AddTransient<ISitecoreBotService, SitecoreBotService>();
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
