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

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}
