using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SC90.Bot.Infrastructure.Rules;

namespace SC90.Bot.Infrastructure.Interfaces
{
    public class DialogActionContext
    {
        public DialogActionContext(IDialogContext context, IDialog dialog)
        {
            Context = context;
            Dialog = dialog;
            ConversationCompleted = false;
        }

        public IDialogContext Context { get; set; }
        public IDialog Dialog { get; set; }
        public bool ConversationCompleted { get; set; }        
        public DialogStateContext ActionState { get; set; }
    }
}