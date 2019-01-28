using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace SC90.Bot.Infrastructure
{
    public interface IDialogAction
    {
        bool IsPromptDialog { get; }
    }
}