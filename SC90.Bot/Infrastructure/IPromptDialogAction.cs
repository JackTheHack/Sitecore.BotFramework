using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;

namespace SC90.Bot.Infrastructure
{
    public interface IPromptDialogAction : IDialogAction
    {
        Task Execute(DialogActionContext context, ResumeAfter<object> resumeAction);
        Task HandleDialogResult(IDialogContext context, object dialogResult);
    }
}