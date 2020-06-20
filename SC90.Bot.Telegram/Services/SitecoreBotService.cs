using System;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using MongoDB.Bson;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Constants;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;

namespace SC90.Bot.Telegram.Services
{
    public class SitecoreBotService : ISitecoreBotService
    {
        private readonly ISessionProvider _session;
        private readonly ISitecoreContext _sitecoreContext;
        private readonly ITelegramService _telegramService;
        private readonly IRuleEngineService _ruleEngine;
        private readonly ICommandService _commandService;

        private _Bot _chatBot;

        private _State _currentState;
        private _State _globalState;

        private BsonDocument _sessionDocument;
        private ChatUpdate _chatUpdate;

        public SitecoreBotService(ISessionProvider session, ISitecoreContext sitecoreContext, 
            ITelegramService telegramService, IRuleEngineService ruleEngineService,
            ICommandService commandService)
        {
            _session = session;
            _sitecoreContext = sitecoreContext;
            _telegramService = telegramService;
            _commandService = commandService;
            _ruleEngine = ruleEngineService;
        }

        public async Task HandleUpdate(ChatUpdate update, _Bot chatBotItem)
        {
            Sitecore.Diagnostics.Log.Info("Running chatbot update", this);

            var sessionKey = $"{update.Source}{update.UserId}";
            
            _chatBot = chatBotItem;
            _chatUpdate = update;
            _sessionDocument = await _session.GetSessionDocument(sessionKey);

            var startDialog = _sitecoreContext.GetItem<_Dialogue>(chatBotItem.Start);

            if (startDialog == null)
            {
                throw new InvalidOperationException("Start dialog is not set.");
            }

            if (_sessionDocument == null)
            {
                _sessionDocument = new BsonDocument();
            }

            _currentState = GetStateOrDefault();

            Sitecore.Diagnostics.Log.Info($"Current chatbot state - {_currentState.Id}", this);

            _globalState = _sitecoreContext.GetItem<_State>(_chatBot.GlobalState);

            await _session.UpdateSessionDocument(sessionKey, _sessionDocument);

            var command = GetStateCommand();

            if (command == null)
            {
                Sitecore.Diagnostics.Log.Warn($"No command found for the chat request.", this);
                return;
            }

            var commandContext = new ChatbotCommandContext()
            {
                Chatbot = _chatBot,
                ChatUpdate = _chatUpdate,
                CommandContext = command,
                CurrentState = _currentState
            };
            
            Sitecore.Diagnostics.Log.Info($"Running command - {command.Id}", this);

            await _commandService.Execute(command, commandContext);
        }

        private _Command GetStateCommand()
        {
            if (_currentState?.Commands != null)
            {
                foreach (var stateCommand in _currentState.Commands)
                {
                    var ruleEngineContext = new ChatbotRuleContext()
                    {
                        Chatbot = _chatBot,
                        CurrentState = _currentState,
                        CommandContext = stateCommand,
                        ChatUpdate = _chatUpdate,
                        
                    };

                    var commandItem = _sitecoreContext.GetItem<Item>(stateCommand.Id);

                    if (_ruleEngine.RunRules(commandItem, I_CommandConstants.RuleFieldName, ruleEngineContext))
                    {
                        return stateCommand;
                    }
                }
            }

            //fallback to global state
            if (_globalState?.Commands != null)
            {
                foreach (var globalStateCommand in _globalState.Commands)
                {
                    var ruleEngineContext = new ChatbotRuleContext()
                    {
                        Chatbot = _chatBot,
                        CurrentState = _currentState,
                        CommandContext = globalStateCommand,
                        ChatUpdate = _chatUpdate,
                        
                    };

                    var commandItem = _sitecoreContext.GetItem<Item>(globalStateCommand.Id);

                    if (_ruleEngine.RunRules(commandItem, I_CommandConstants.RuleFieldName, ruleEngineContext))
                    {
                        globalStateCommand.IsGlobal = true;
                        return globalStateCommand;
                    }
                }
            }

            return null;
        }

        private _State GetStateOrDefault()
        {
            var defaultDialogueItem = _sitecoreContext.GetItem<_Dialogue>(_chatBot.Start);
            var defaultStateItem = _sitecoreContext.GetItem<_State>(defaultDialogueItem.NewState);

            if (_sessionDocument.TryGetValue(SessionConstants.State, out var stateVal))
            {
                _State stateItem = null;
                
                if (stateVal.AsNullableGuid.HasValue)
                {
                    stateItem = _sitecoreContext.GetItem<_State>(stateVal.AsNullableGuid.Value);
                }

                if (stateItem == null)
                {
                    _sessionDocument.Set(SessionConstants.State, defaultStateItem.Id);
                    return defaultStateItem;
                }

                return stateItem;
            }

            _sessionDocument.Set(SessionConstants.State, defaultStateItem.Id);
            return defaultStateItem;
        }

        public async Task Register(string source, string requestUriHost)
        {
            switch (source)
            {
                case "telegram":
                    var webHookUrl = _telegramService.Configuration.WebHookEndpoint;
                    await _telegramService.Client.SetWebhookAsync(webHookUrl);
                    break;
            }
        }
    }
}
