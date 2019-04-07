using Microsoft.Bot.Builder.Dialogs;

namespace SC90.Bot.Infrastructure.Extensions
{
    public static class BotExtensions
    {
        public static void SetValueOrRemoveIfNull<T>(this IBotDataBag bag, string key, T value)
        {
            if (value == null && bag.ContainsKey(key))
            {
                bag.RemoveValue(key);
            }
            else
            {
                bag.SetValue(key, value);
            }
        }
    }
}