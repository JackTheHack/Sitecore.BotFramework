using System;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface IBotRequestContext
    {
        void SetBotContext(Guid botId, bool isScheduler);
        Guid BotId { get;  }
        bool IsBotCommand { get;  }
    }
}
