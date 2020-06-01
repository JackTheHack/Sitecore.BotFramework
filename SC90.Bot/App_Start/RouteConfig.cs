using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SC90.Bot
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute("BotApi", "sitecore/bot/{botId}/{action}", new
            {
                controller = "Bot",
                action = "Post", 
                id = RouteParameter.Optional
            });            

            routes.MapHttpRoute("TelegramApi", "sitecore/telegram/{botId}/{action}", new
            {
                controller = "Telegram",
                action = "Post", 
                id = RouteParameter.Optional
            });   
        }
    }
}
