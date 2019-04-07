using System.Threading.Tasks;

namespace SC90.Bot.Infrastructure.Interfaces
{
    public interface IMessageDialogAction : IDialogAction
    {
        Task Execute(DialogActionContext context);
    }
}