using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2.Dialogues;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using SC90.Bot.Telegram.Models;
using Sitecore.DependencyInjection;

namespace SC90.Bot.Telegram.Actions
{
    public class RunDialogueAction : IDialogueAction
    {
        private RunDialogue _actionItem;
        
        private readonly ISitecoreService _sitecoreContext;
        private readonly ICommandService _commandService;

        
        private ChatbotActionContext _context;

        public RunDialogueAction()
        {
            _sitecoreContext = new SitecoreService("web");
            _commandService = ServiceLocator.ServiceProvider.GetService<ICommandService>();
        }

        public void SetContextItem(_DialogAction action, ChatbotActionContext context)
        {
            _actionItem = _sitecoreContext.GetItem<RunDialogue>(action.Id);

            _context = context;
        }

        public async Task Execute()
        {
            var dialogue = _sitecoreContext.GetItem<_Dialogue>(_actionItem.Dialog);

            if (dialogue != null)
            {
                var dialogueContext = new ChatbotDialogueContext()
                {
                    Chatbot = _context.Chatbot,
                    ChatUpdate = _context.ChatUpdate,
                    CurrentState = _context.CurrentState,
                    DialogueContext = dialogue,
                    SchedulingData = _context.SchedulingData,
                    SessionKey = _context.SessionKey
                };

                await _commandService.Execute(dialogue, dialogueContext);
            }
        }
    }
}
