using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
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
                FormId = _formId //TODO: Form id here
            };

            FillFields(formSubmitContext, _mappings);

            _formSubmitHandler.Submit(formSubmitContext);
            return Task.CompletedTask;
        }

        private void FillFields(FormSubmitContext formSubmitContext, NameValueListField mappings)
        {
            foreach (string mappingsNameValue in _mappings.NameValues)
            {
                var value = _mappings.NameValues[mappingsNameValue];
                var fieldModel = new TextViewModel()
                {
                    Name = mappingsNameValue,
                    Text = value //TODO: Fill other fields
                };

                //TODO: Do mapping for different types

                formSubmitContext.Fields.Add(fieldModel);
            }
        }

    }
}
