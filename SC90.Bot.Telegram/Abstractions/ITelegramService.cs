using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface ITelegramService
    {
        TelegramBotClient Client { get; }
    }
}
