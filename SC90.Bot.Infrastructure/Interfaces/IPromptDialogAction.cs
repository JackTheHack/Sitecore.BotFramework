using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Infrastructure.Rules;

namespace SC90.Bot.Infrastructure.Interfaces
{
    public interface IPromptDialogAction : IDialogAction
    {
        Task Execute(DialogActionContext context, ResumeAfter<object> resumeAction);
        Task HandleDialogResult(IDialogContext context, DialogStateContext dialogStateContext, object dialogResult);
    }
}