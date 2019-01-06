using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.ChatBot;

namespace SC90.Bot
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RouteTable.Routes.MapHttpRoute("BotApi", "sitecore/api/bot/{action}/{id}", new
            {
                controller = "Bot",
                action = "Post"
            });

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
