using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2.Actions;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SC90.Bot.Telegram.Actions
{
    public class ConditionAction : IDialogueAction
    {
        private Condition _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ITelegramService _telegramService;
        private readonly IRuleEngineService _ruleEngineService;
        private readonly IDialogActionFactory _actionFactory;

        
        private ChatbotActionContext _context;


        public ConditionAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _actionFactory = ServiceLocator.ServiceProvider.GetService<IDialogActionFactory>();
            _telegramService = ServiceLocator.ServiceProvider.GetService<ITelegramService>();
            _ruleEngineService = ServiceLocator.ServiceProvider.GetService<IRuleEngineService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<Condition>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            var item = _sitecoreContext.GetItem<Item>(_actionItem.Id);
            
            var conditionRuleContext = new ChatbotRuleContext()
            {
                Chatbot = _context.Chatbot,
                ChatUpdate = _context.ChatUpdate,
                CommandContext =  _context.CommandContext,
                CurrentState = _context.CurrentState,
                SessionId = _context.SessionKey
            };

            if (_ruleEngineService.RunRules(item, IConditionConstants.RuleFieldName, conditionRuleContext))
            {
                if (conditionRuleContext.Result &&
                    !string.IsNullOrEmpty(conditionRuleContext.SelectedDecisionBranch))
                {
                    //get selected condition
                    
                    var decisionBranchItem = _actionItem.DecisionBranches?.FirstOrDefault(i => i.Name == conditionRuleContext.SelectedDecisionBranch);

                    if (decisionBranchItem == null)
                    {
                        return;
                    }

                    foreach (var action in decisionBranchItem.Actions)
                    {
                        Sitecore.Diagnostics.Log.Info($"Running action - {action.Id}", this);

                        var actionHandler = _actionFactory.CreateHandler(action);

                        if (actionHandler != null)
                        {
                            actionHandler.SetContextItem(action, _context);
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
    }
}
