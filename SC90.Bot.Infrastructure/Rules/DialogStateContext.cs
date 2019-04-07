using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC90.Bot.Infrastructure.Rules
{
    [Serializable]
    public class DialogStateContext
    {
        public string SelectedDecisionBranchFlow { get; set; }
        public string SelectedDialogFlow { get; set; }
    }
}