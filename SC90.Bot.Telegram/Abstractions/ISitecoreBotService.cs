using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Models;
using Task = System.Threading.Tasks.Task;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ISitecoreBotService
    {
        Task HandleUpdate(ChatUpdate update, _Bot chatBotItem);
        Task Register(string uriHost, string requestUriHost);
    }
}
