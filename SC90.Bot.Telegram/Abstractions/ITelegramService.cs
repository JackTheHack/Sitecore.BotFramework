using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.Telegram.Models;
using Telegram.Bot;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ITelegramService
    {
        TelegramBotConfiguration Configuration { get; set; }
        TelegramBotClient Client { get; }
    }
}
