using System;
using System.Collections.Generic;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Feature.SitecoreBotFrameworkV2;
using SC90.Bot.CodeGen.SC90.Bot.CodeGen.sitecore.templates.Foundation.SitecoreBotFrameworkV2;
using SC90.Bot.Telegram.Abstractions;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;

namespace SC90.Bot.Telegram.Services
{
    public class DialogActionFactory : IDialogActionFactory
    {
        public static readonly Dictionary<Guid, Type> MappingDictionary = new Dictionary<Guid, Type>();

        static DialogActionFactory()
        {
            LoadMappings();
        }

        public static void LoadMappings()
        {
            try
            {
                var mappings = Factory.GetConfigNodes("sitecoreBotFramework/actionMap/actionMapping");

                for (int i = 0; i < mappings.Count; i++)
                {
                    var mappingItem = mappings.Item(i);
                    if (mappingItem?.Attributes != null)
                    {
                        Guid.TryParse(mappingItem.Attributes["id"].Value, out var mappingId);
                        string type = mappingItem.Attributes["type"].Value;
                        MappingDictionary.Add(mappingId, Type.GetType(type));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e, typeof(DialogActionFactory));
            }
        }

        public IDialogueAction CreateHandler(_DialogAction actionItem)
        {
            if (!MappingDictionary.ContainsKey(actionItem.TemplateId))
            {
                throw new InvalidOperationException("Mapping for this bot action template not found.");
            }

            var mappingType = MappingDictionary[actionItem.TemplateId];

            return Activator.CreateInstance(mappingType) as IDialogueAction;
        }
    }
}