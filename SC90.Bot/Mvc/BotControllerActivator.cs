using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using SC90.Bot.Controllers;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace SC90.Bot.Mvc
{
    public class BotControllerActivator : IHttpControllerActivator
    {
        private readonly DefaultHttpControllerActivator  _defaultHttpControllerFactory;

        public BotControllerActivator()
        {
            _defaultHttpControllerFactory = new DefaultHttpControllerActivator();
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var routeData = request.GetRouteData();

            if (Sitecore.Context.Site != null &&
                routeData.Values.ContainsKey("botId") &&
                !string.IsNullOrEmpty((string)routeData.Values["botId"]))
            {
                var botName = routeData.Values["botId"];
                var siteItemPath = Sitecore.Context.Site.ContentStartPath;

                var botStartItemId = GetBotStartItem(botName, siteItemPath);

                if (botStartItemId == null)
                {
                    return _defaultHttpControllerFactory.Create(request, controllerDescriptor, controllerType);
                }


                var instance = Activator.CreateInstance(controllerType, botStartItemId);
                return (IHttpController)instance;
            }

            return _defaultHttpControllerFactory.Create(request, controllerDescriptor, controllerType);
        }

        private Item GetBotStartItem(object botName, string site)
        {
            var botFolder = Settings.GetSetting("BotFramework:BotFolder", "/Data/Bots");

            var botItemsRoot = Sitecore.Context.Database.GetItem(site + botFolder);

            if (botItemsRoot == null)
            {
                throw new ArgumentException("Bot root item not found.");
            }

            var botItem = botItemsRoot.Children
                .FirstOrDefault(i => i["Route"] == (string) botName);

            return botItem;


        }
    }
}