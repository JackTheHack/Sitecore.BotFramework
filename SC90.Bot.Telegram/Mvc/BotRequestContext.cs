using SC90.Bot.Telegram.Abstractions;
using System;
using System.Web;

namespace SC90.Bot.Telegram.Mvc
{
    public class BotRequestContext : IBotRequestContext
    {
        private Guid _botId;
        private bool _isScheduler = false;

        public Guid BotId { get { 
                if(HttpContext.Current?.Items?["botId"]!= null &&
                    Guid.TryParse(HttpContext.Current.Items["botId"].ToString(), out var botIdVal))
                {
                    return botIdVal;
                }

                return _botId;
            } 
        }

        public bool IsScheduler { get { return _isScheduler; } }

        public void SetBotContext(Guid botId, bool isScheduler)
        {
            _botId = botId;
            _isScheduler = isScheduler;

            if(HttpContext.Current?.Items != null)
            {
                HttpContext.Current.Items["botId"] = botId;
            }
        }
    }
}
