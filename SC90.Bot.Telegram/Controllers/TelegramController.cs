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
using Sitecore.Services.Infrastructure.Web.Http;
using Telegram.Bot.Types;

namespace SC90.Bot.Telegram.Controllers
{
    public class TelegramApiController : ApiController
    {
        private readonly ISitecoreContext _sitecoreContext;
        
        private readonly _Bot _chatBotItem;
        private _Dialogue _startDialog;
        private readonly ISitecoreBotService _sitecoreBotService;

        public TelegramApiController(Item startItem)
        {
            _sitecoreContext = ServiceLocator.ServiceProvider.GetService<ISitecoreContext>();
            _sitecoreBotService = ServiceLocator.ServiceProvider.GetService<ISitecoreBotService>();
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
                await _sitecoreBotService.HandleUpdate(chatUpdate, _chatBotItem);
                
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
            await _sitecoreBotService.Register("telegram", RequestContext.Url.Request.RequestUri.Host);
            return Content(HttpStatusCode.OK, "Registered.");
        }
    }
}