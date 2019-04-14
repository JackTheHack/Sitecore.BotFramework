using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Forms.Actions.Helpers;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Mvc.Pipelines.RenderFields;
using Sitecore.ExperienceForms.Processing;

namespace SC90.Bot.Forms.Actions.Infrastructure.DialogActions
{
    [Serializable]
    public class SubmitForm : IMessageDialogAction
    {
        [NonSerialized]
        private readonly Item _dialogAction;
        private readonly Guid _formId;
        private readonly IFormSubmitHandler _formSubmitHandler;
        private readonly NameValueListField _mappings;
        private Item _formItem;
        private Dictionary<string, Item> _fieldDictionary;

        public SubmitForm(Item dialogAction)
        {
            _dialogAction = dialogAction;
            Guid.TryParse(_dialogAction["FormId"], out _formId);
            _mappings = (NameValueListField)_dialogAction.Fields["Mappings"];
            _formSubmitHandler = ServiceLocator.ServiceProvider.GetService(typeof(IFormSubmitHandler)) as IFormSubmitHandler;
        }

        public bool IsPromptDialog => false;

        public Task Execute(DialogActionContext context)
        {
            var formSubmitContext = new FormSubmitContext("Submit")
            {
                SessionId = Guid.Empty,
                Fields = new List<IViewModel>(),
                FormId = _formId
            };

            _formItem = Sitecore.Context.Database.GetItem(ID.Parse(_formId));

            FillFields(context, formSubmitContext);

            _formSubmitHandler.Submit(formSubmitContext);

            return Task.CompletedTask;
        }

        private void FillFields(DialogActionContext submitContext, FormSubmitContext formSubmitContext)
        {
            foreach (string mappingsNameValue in _mappings.NameValues)
            {
                if (!_fieldDictionary.ContainsKey(mappingsNameValue))
                {
                    continue;
                }

                var variableName = _mappings.NameValues[mappingsNameValue];

                if (!submitContext.Context.PrivateConversationData.TryGetValue<string>("VAR_" + variableName,
                    out var variableValue))
                {
                    continue;
                };

                var fieldItem = _fieldDictionary[variableName];

                var fieldType = ((LinkField)fieldItem.Fields["Field Type"]).TargetItem;

                var viewModel = InstanceHelper.CreateInstance(fieldType["Model Type"]) as IViewModel;

                if (viewModel == null)
                {
                    continue;
                }

                viewModel.InitializeModel(fieldItem);

                var fieldModel = viewModel as InputViewModel<string>;

                if (fieldModel == null)
                {
                    continue;
                }

                fieldModel.Value = variableValue;

                formSubmitContext.Fields.Add(fieldModel);
            }
        }
    }
}
