using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using Sitecore.Data.Items;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface IDialogActionFactory
    {
        IDialogueAction CreateHandler(_DialogAction actionItem);
    }
}
