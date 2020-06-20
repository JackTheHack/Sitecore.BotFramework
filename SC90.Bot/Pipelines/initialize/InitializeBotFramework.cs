using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace SC90.Bot.Pipelines.initialize
{
    public class InitializeBotFramework
    {
        public void Process(PipelineArgs args)
        {
            Log.Info("Initializing BotFramework", this);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}