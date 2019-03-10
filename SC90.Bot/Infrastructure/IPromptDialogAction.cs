using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Infrastructure.Rules;

namespace SC90.Bot.Infrastructure
{
    public interface IPromptDialogAction : IDialogAction
    {
        Task Execute(DialogActionContext context, ResumeAfter<object> resumeAction);
        Task HandleDialogResult(IDialogContext context, DialogStateContext dialogStateContext, object dialogResult);
    }
}