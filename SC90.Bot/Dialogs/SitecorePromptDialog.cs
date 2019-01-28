using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Sitecore.Data.Items;

namespace SC90.Bot.Dialogs
{
    [Serializable]
    public class SitecorePromptDialog : IDialog<object>
    {
        private int _attempts;
        private readonly string _promptMessage;
        private readonly string _validationMessage;

        public SitecorePromptDialog(Item promptDialogItem)
        {
            _promptMessage = promptDialogItem["Message"];
            _validationMessage = promptDialogItem["ValidationMessage"];
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(_promptMessage);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            /* If the message returned is a valid name, return it to the calling dialog. */
            if (ValidateInput(message))
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.Done(message.Text);
            }
            /* Else, try again by re-prompting the user. */
            else
            {
                --_attempts;

                if (_attempts > 0)
                {
                    await context.PostAsync(_validationMessage);

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                        parent/calling dialog. */
                    context.Fail(new TooManyAttemptsException("Validation failed."));
                }
            }
        }

        protected virtual bool ValidateInput(IMessageActivity message)
        {
            return (message.Text != null) && (message.Text.Trim().Length > 0);
        }
    }
}