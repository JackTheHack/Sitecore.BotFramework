using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;

namespace SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2.Actions
{
    public partial class Condition
    {
        [SitecoreChildren(TemplateId = I_DecisionBranchConstants.TemplateIdString)]
        public virtual IEnumerable<_DecisionBranch> DecisionBranches {get; set;}
    }
}
