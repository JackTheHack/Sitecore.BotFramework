using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Project.Sandbox;
using Sitecore.Shell.Framework.Commands;

namespace SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2
{
    public partial class _State
    {
        [SitecoreChildren(TemplateId = I_CommandConstants.TemplateIdString)]
        public virtual IEnumerable<_Command> Commands {get; set;}
    }
}
