using System.Web.Http;
using System.Web.Http.Dispatcher;
using BotControllerActivator = SC90.Bot.Mvc.BotControllerActivator;

namespace SC90.Bot
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Json settings
            //config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    Formatting = Newtonsoft.Json.Formatting.Indented,
            //    NullValueHandling = NullValueHandling.Ignore,
            //};

            // Web API configuration and services

            // Web API routes
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Services.Replace(typeof(IHttpControllerActivator), new BotControllerActivator());
            
        }
    }
}
