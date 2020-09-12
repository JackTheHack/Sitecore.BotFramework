using System;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Quartz;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Constants;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Log = Sitecore.Diagnostics.Log;

namespace SC90.Bot.Telegram.Services
{
    public class SitecoreBotService : ISitecoreBotService
    {
        private readonly ICommandService _commandService;
        private readonly IRuleEngineService _ruleEngine;
        private readonly ISessionProvider _session;
        private readonly ISitecoreService _sitecoreContext;
        private readonly ITelegramService _telegramService;

        private _Bot _chatBot;
        private ChatUpdate _chatUpdate;

        private _State _currentState;
        private _State _globalState;
        private string _sessionId;
        private BsonDocument _sessionDocument;
        private IDialogActionFactory _actionFactory;

        public SitecoreBotService(ISessionProvider session, 
            ITelegramService telegramService, IRuleEngineService ruleEngineService,
            IDialogActionFactory actionFactory,
            ICommandService commandService)
        {
            _session = session;
            _actionFactory = actionFactory;
            _sitecoreContext = new SitecoreService(Sitecore.Data.Database.GetDatabase("web"));
            _telegramService = telegramService;
            _commandService = commandService;
            _ruleEngine = ruleEngineService;
        }

        public async Task HandleSchedulingJob(IJobExecutionContext context)
        {
            var jobData = new SchedulingJobData(context.Trigger.JobDataMap);

            if(jobData.BotId == Guid.Empty || string.IsNullOrEmpty(jobData.SessionId))
            {
                return;
            }
            
            var bot = _sitecoreContext.GetItem<_Bot>(jobData.BotId);

            _sessionId = jobData.SessionId;

            var sessionDocument = await _session.GetSessionDocument(jobData.SessionId);

            _globalState = _sitecoreContext.GetItem<_State>(bot.GlobalState);

            _currentState = 
                sessionDocument.TryGetValue(SessionConstants.State, out var stateVal) && stateVal.AsNullableGuid.HasValue ? 
                    _sitecoreContext.GetItem<_State>(stateVal.AsNullableGuid.Value) : 
                    _sitecoreContext.GetItem<_State>(bot.Start);

            if (_currentState == null)
            {
                throw new InvalidOperationException("Can't find current state for scheduling job.");
            }

            var command = GetStateCommand(_currentState, jobData);

            if (command == null)
            {
                Log.Warn("No command found for the scheduling job.", this);
                return;
            }

            var commandContext = new ChatbotCommandContext
            {
                Chatbot = _chatBot,
                ChatUpdate = jobData.IsGlobal ? null : new ChatUpdate() { Message = null, Source = jobData.Source, UserId = jobData.UserId },
                CommandContext = command,
                CurrentState = _currentState,
                SchedulingData = jobData,
                SessionKey = jobData.SessionId
            };

            Log.Info($"Running command - {command.Id}", this);

            await _commandService.Execute(command, commandContext);
        }

        public async Task HandleUpdate(ChatUpdate update, _Bot chatBotItem)
        {
            var sessionKey = GetSessionKey(update.Source, update.UserId);

            _chatBot = chatBotItem;
            _chatUpdate = update;

            Log.Info("Running chatbot update", this);
            _sessionId = sessionKey;
            _sessionDocument = await _session.GetSessionDocument(sessionKey);

            try
            {
                var startDialog = _sitecoreContext.GetItem<_Dialogue>(chatBotItem.Start);

                if (startDialog == null) throw new InvalidOperationException("Start dialog is not set.");

                if (_sessionDocument == null) _sessionDocument = new BsonDocument();

                _currentState = GetStateOrDefault();

                Log.Info($"Current chatbot state - {_currentState.Id}", this);

                _globalState = _sitecoreContext.GetItem<_State>(_chatBot.GlobalState);

                if (await RunDebugCommandsIfRequired()) return;

                await _session.UpdateSessionDocument(sessionKey, _sessionDocument);

                var command = GetStateCommand(_currentState);

                if (command == null)
                {
                    Log.Warn("No command found for the chat request.", this);
                    return;
                }

                var commandContext = new ChatbotCommandContext
                {
                    Chatbot = _chatBot,
                    ChatUpdate = _chatUpdate,
                    CommandContext = command,
                    CurrentState = _currentState,
                    SessionKey = sessionKey
                };

                Log.Info($"Running command - {command.Id}", this);

                await _commandService.Execute(command, commandContext);
            }
            catch (Exception e)
            {
                Log.Error("Chatbot update failed", e, this);

                if (await PrintDebugExceptionDetailsIfRequired(e)) return;

                throw;
            }
        }

        private static string GetSessionKey(string source, string userId)
        {
            return $"{source}{userId}";
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

        private async Task<bool> PrintDebugExceptionDetailsIfRequired(Exception e)
        {
            if (_chatBot.EnableDebugMode)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"EXCEPTION - {e}");
                sb.AppendLine($"SESSION - {_sessionDocument}");
                sb.AppendLine($"STATE - *{_currentState?.Name}* {_currentState?.FullPath}");
                sb.AppendLine($"GLOBAL STATE - *{_globalState?.Name}* {_globalState?.FullPath}");
                await _telegramService.Client.SendTextMessageAsync(_chatUpdate.UserId, sb.ToString());
                return true;
            }

            return false;
        }

        private async Task<bool> RunDebugCommandsIfRequired()
        {
            if (_chatBot.EnableDebugMode)
                if (_chatUpdate.Message == "/debuginfo")
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"SESSION - {_sessionDocument}");
                    sb.AppendLine($"STATE - *{_currentState?.Name}* {_currentState?.FullPath}");
                    sb.AppendLine($"GLOBAL STATE - *{_globalState?.Name}* {_globalState?.FullPath}");
                    await _telegramService.Client.SendTextMessageAsync(_chatUpdate.UserId,
                        sb.ToString());
                    return true;
                }

            return false;
        }

        protected virtual _Command GetStateCommand(_State currentState, SchedulingJobData schedulingData = null)
        {
            if (_currentState?.Commands != null)
                foreach (var stateCommand in currentState.Commands)
                {
                    var ruleEngineContext = new ChatbotRuleContext
                    {
                        Chatbot = _chatBot,
                        CurrentState = currentState,
                        CommandContext = stateCommand,
                        ChatUpdate = _chatUpdate,
                        SchedulingData = schedulingData,
                        SessionId = _sessionId
                    };

                    var commandItem = _sitecoreContext.GetItem<Item>(stateCommand.Id);

                    if (_ruleEngine.RunRules(commandItem, I_CommandConstants.RuleFieldName, ruleEngineContext))
                        return stateCommand;
                }

            //fallback to global state
            if (_globalState?.Commands != null)
                foreach (var globalStateCommand in _globalState.Commands)
                {
                    var ruleEngineContext = new ChatbotRuleContext
                    {
                        Chatbot = _chatBot,
                        CurrentState = currentState,
                        CommandContext = globalStateCommand,
                        ChatUpdate = _chatUpdate,
                        SessionId = _sessionId
                    };

                    var commandItem = _sitecoreContext.GetItem<Item>(globalStateCommand.Id);

                    if (_ruleEngine.RunRules(commandItem, I_CommandConstants.RuleFieldName, ruleEngineContext))
                    {
                        globalStateCommand.IsGlobal = true;
                        return globalStateCommand;
                    }
                }

            return null;
        }

        protected virtual _State GetStateOrDefault()
        {
            var defaultDialogueItem = _sitecoreContext.GetItem<_Dialogue>(_chatBot.Start);
            var defaultStateItem = _sitecoreContext.GetItem<_State>(defaultDialogueItem.NewState);

            if (_sessionDocument.TryGetValue(SessionConstants.State, out var stateVal))
            {
                _State stateItem = null;

                if (stateVal.AsNullableGuid.HasValue)
                    stateItem = _sitecoreContext.GetItem<_State>(stateVal.AsNullableGuid.Value);

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
    }
}