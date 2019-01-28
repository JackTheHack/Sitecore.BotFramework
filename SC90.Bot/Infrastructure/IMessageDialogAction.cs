using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SC90.Bot.Infrastructure
{
    public interface IMessageDialogAction : IDialogAction
    {
        Task Execute(DialogActionContext context);
    }
}