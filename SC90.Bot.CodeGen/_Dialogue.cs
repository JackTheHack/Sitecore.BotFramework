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
    public partial class _Dialogue
    {
        [SitecoreQuery("query:./Actions/*")]
        public virtual IEnumerable<_Command> Actions {get; set;}

        [SitecoreQuery("query:./States/*")]
        public virtual IEnumerable<_Command> States {get; set;}
    }
}
