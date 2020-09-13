using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2
{
    public partial class _State
    {
        [SitecoreChildren(TemplateId = I_CommandConstants.TemplateIdString)]
        public virtual IEnumerable<_Command> Commands {get; set;}
    }
}
