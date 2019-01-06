using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Sitecore.ChatBot.Dialogs
{
    [LuisModel("eb2abdfb-ade9-4dbf-96f7-3eda7b7596a6", "095cef5a0a844442a009402eff0436ff")]
    [Serializable]
    public partial class LuisIntentDialog : LuisDialog<object>
    {
        private const string TimePeriodEntityPrefix = "builtin.datetime";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand :(";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("View Total Requests")]
        public async Task ViewTotalRequests(IDialogContext context, LuisResult result)
        {
            var entity = result.Entities.FirstOrDefault(x => x.Type.StartsWith(TimePeriodEntityPrefix));

            if (entity != null && entity.Type.StartsWith(TimePeriodEntityPrefix))
            {
                string customPeriod = entity.Resolution.FirstOrDefault().Value?.ToString();

                if (DateTime.TryParse(customPeriod, out var dateTime))
                {
                    // A date was specified, not a duration, so transform to a time period
                    customPeriod = $"{customPeriod}/{(dateTime + TimeSpan.FromDays(1)):yyyy-MM-dd}";
                }
                
                var count = 0;

                await context.PostAsync($"There have been {count} requests to the server in the requested timeframe ({customPeriod})");
                context.Wait(MessageReceived);
            }
            else
            {
                var count = 1;

                await context.PostAsync($"There have been a total of {count} requests to the server.");
                context.Wait(MessageReceived);
            }

            
        }
    }
}