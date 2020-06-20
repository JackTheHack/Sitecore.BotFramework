using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC90.Bot.Telegram.Models;
using Sitecore.Data.Items;

namespace SC90.Bot.Telegram.Abstractions
{
    public interface IRuleEngineService
    {
        bool RunRules<T>(Item item, string fieldName, T ruleContext)
            where T : ChatbotRuleContext;
    }
}
