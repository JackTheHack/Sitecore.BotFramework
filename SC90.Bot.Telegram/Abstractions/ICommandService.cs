using System;
using System.Threading.Tasks;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Models;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ICommandService
    {
        Task Execute(_Command commandId, ChatbotCommandContext context);
    }
}