using System.Threading.Tasks;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;

namespace SC90.Bot.Telegram.Services
{
    public class CommandService : ICommandService
    {
        private readonly IDialogActionFactory _actionFactory;

        public CommandService(IDialogActionFactory actionFactory)
        {
            _actionFactory = actionFactory;
        }

        public async Task Execute(_Command command, ChatbotCommandContext context)
        {
            foreach (var action in command.Actions)
            {
                Sitecore.Diagnostics.Log.Info($"Running action - {action.Id}", this);

                var actionContext = new ChatbotActionContext()
                {
                    ChatUpdate = context.ChatUpdate,
                    CurrentState = context.CurrentState,
                    CommandContext = context.CommandContext,
                    Chatbot = context.Chatbot,
                    ActionContext = action
                };

                var actionHandler = _actionFactory.CreateHandler(action);

                if (actionHandler != null)
                {
                    actionHandler.SetContextItem(action, actionContext);
                    await actionHandler.Execute();
                }
                else
                {
                    Sitecore.Diagnostics.Log.Warn($"Action handler not found - {action.Id}", this);
                }
            }
        }
    }
}
