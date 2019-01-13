using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SC90.Bot.Infrastructure.DialogActions;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SC90.Bot.Infrastructure
{
    public class DialogActionFactory
    {
        public static IDialogAction CreateHandler(Item actionItem)
        {
            switch (actionItem.TemplateID.Guid.ToString())
            {
                case "78aa458b-f1f1-4845-802f-50abc14ebd35":
                    return new SendMessage(actionItem);
            }

            throw new NotImplementedException("Dialog action not found.");
        }
    }
}