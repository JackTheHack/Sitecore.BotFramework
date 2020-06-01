using System.Threading.Tasks;
using System.Web.Http;
using Sitecore.Services.Infrastructure.Web.Http;
using Telegram.Bot.Types;

namespace SC90.Bot.Controllers
{
    public class TelegramController : ServicesApiController
    {
        // POST api/update
        [HttpPost]
        public async Task<IHttpActionResult > Post([FromBody]Update update)
        {
            return Ok();
        }   
    }
}