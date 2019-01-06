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
            routes.MapHttpRoute("CommentApi", "sitecore/api/bot/{id}", new
            {
                controller = "CommentApi",
                action = "Get"
            });
        }
    }
}
