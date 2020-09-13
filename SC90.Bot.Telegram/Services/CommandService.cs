using System;
using System.Linq;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Constants;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;

namespace SC90.Bot.Telegram.Services
{
    public class CommandService : ICommandService
    {
        private readonly IDialogActionFactory _actionFactory;
        private readonly ISessionProvider _session;
        private readonly ISitecoreService _sitecore;

        public CommandService(IDialogActionFactory actionFactory, ISessionProvider session)
        {
            _actionFactory = actionFactory;
            _session = session;
            _sitecore = new SitecoreService("web");
        }

        public async Task Execute(_DialogAction action, ChatbotActionContext context)
        {
            var actionHandler = _actionFactory.CreateHandler(action);

            if (actionHandler != null)
            {
                actionHandler.SetContextItem(action, context);
                await actionHandler.Execute();
            }
            else
            {
                Sitecore.Diagnostics.Log.Warn($"Action handler not found - {action.Id}", this);
            }
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
                    ActionContext = action,
                    SessionKey = context.SessionKey,
                    SchedulingData = context.SchedulingData
                };

                await Execute(action, actionContext);
            }
        }

        public async Task Execute(_Dialogue dialogue, ChatbotDialogueContext context)
        {
            //Execute all dialogue actions first
            var dialogueItem = _sitecore.GetItem<Item>(dialogue.Id);
            var actionsItem = dialogueItem.Children["Actions"];

            if (actionsItem != null)
            {
                var actions = actionsItem.Children.ToArray().Select(i => _sitecore.GetItem<_DialogAction>(i));

                foreach (var command in actions)
                {
                    if (command == null) continue;

                    var commandContext = new ChatbotActionContext()
                    {
                        Chatbot = context.Chatbot,
                        ChatUpdate = context.ChatUpdate,
                        ActionContext= command,
                        CurrentState = context.CurrentState,
                        SessionKey = context.SessionKey,
                        DialogueContext = dialogue,
                        SchedulingData = context.SchedulingData
                    };

                    await Execute(command, commandContext);
                }
            }

            if(dialogue != null && dialogue.NewState != Guid.Empty)
            {
                await _session.Set(context.SessionKey, SessionConstants.State, dialogue.NewState);
            }
        }
    }
}
