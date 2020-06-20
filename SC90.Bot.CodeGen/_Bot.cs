using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2
{
    public partial class _Bot
    {
        [SitecoreChildren(TemplateId = I_DialogueConstants.TemplateIdString)]
        public virtual IEnumerable<_Dialogue> Dialogs {get; set;}

        [SitecoreChildren(TemplateId = I_StateConstants.TemplateIdString)]
        public virtual IEnumerable<_State> States {get; set;}
    }
}
