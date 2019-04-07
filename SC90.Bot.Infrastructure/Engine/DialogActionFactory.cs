using System;
using System.Collections.Generic;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace SC90.Bot.Infrastructure.Engine
{
    public class DialogActionFactory
    {
        private static readonly Dictionary<ID, Type> MappingDictionary = new Dictionary<ID, Type>();

        static DialogActionFactory()
        {
            LoadMappings();
        }

        private static void LoadMappings()
        {
            try
            {
                var mappings = Factory.GetConfigNodes("sitecoreBotFramework/actionMappings/actionMapping");

                for (int i = 0; i < mappings.Count; i++)
                {
                    var mappingItem = mappings.Item(i);
                    if (mappingItem != null)
                    {
                        ID.TryParse(mappingItem.Attributes["id"].Value, out var mappingId);
                        MappingDictionary.Add(mappingId, Type.GetType(mappingItem.Attributes["type"].Value));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e, typeof(DialogActionFactory));
            }
        }

        public static IDialogAction CreateHandler(Item actionItem)
        {
            if (!MappingDictionary.ContainsKey(actionItem.TemplateID))
            {
                throw new InvalidOperationException("Mapping for this bot action template not found.");
            }

            var mappingType = MappingDictionary[actionItem.TemplateID];

            return Activator.CreateInstance(mappingType, actionItem) as IDialogAction;
        }
    }
}