using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Telegram.Bot.Types;

namespace SC90.Bot.Telegram.Controllers
{
    public class TelegramApiController : ApiController
    {
        private readonly ISitecoreService _sitecoreContext;
        private readonly _Bot _chatBotItem;
        private _Dialogue _startDialog;
        private readonly ISitecoreBotService _sitecoreBotService;
        private readonly ISchedulerService _schedulerService;

        public TelegramApiController(Item startItem)
        {
            _sitecoreContext = new SitecoreService("web");
            _sitecoreBotService = ServiceLocator.ServiceProvider.GetService<ISitecoreBotService>();
            _schedulerService = ServiceLocator.ServiceProvider.GetService<ISchedulerService>();
            _chatBotItem = _sitecoreContext.GetItem<_Bot>(startItem);
        }

        // POST api/update
        [HttpPost]
        public async Task<IHttpActionResult > Post([FromBody]Update update)
        {
            try
            {
                Sitecore.Diagnostics.Log.Info("Handling Telegram chat update", this);
                var chatUpdate = new ChatUpdate()
                {
                    UserId = update.Message.From.Id.ToString(),
                    Source = "telegram",
                    Message = update.Message.Text
                };
                await _sitecoreBotService.HandleUpdate(chatUpdate, _chatBotItem).ConfigureAwait(false);
                
                return Content(HttpStatusCode.OK, "{}");
            }
            catch (Exception e)
            {
                Sitecore.Diagnostics.Log.Error("Error during chatbot update handling", e, this);
                return InternalServerError(e);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> Ping()
        {
            return Content(HttpStatusCode.OK, "Hello!");
        }

        [HttpGet]
        public async Task<IHttpActionResult> Register()
        {
            string botName = RequestContext.RouteData.Values["botId"]?.ToString();

            Assert.IsNotNullOrEmpty(botName, "Bot name can't be empty");

            await _sitecoreBotService.Register("telegram", botName, RequestContext.Url.Request.RequestUri.Host);
            return Content(HttpStatusCode.OK, "Registered.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> ClearJobs()
        {
            await _schedulerService.ClearJobs(_chatBotItem.Id.ToString());
            return Content(HttpStatusCode.OK, "Jobs Cleared.");
        }
    }
}