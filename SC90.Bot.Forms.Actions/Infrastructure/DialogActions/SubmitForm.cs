using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SC90.Bot.Forms.Actions.Helpers;
using SC90.Bot.Infrastructure.Interfaces;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using Sitecore.ExperienceForms.Processing;
using System.Web.Mvc;
using SC90.Bot.Forms.Actions.Models;
using Sitecore.ContentSearch;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Mvc.Models;
using Sitecore.ExperienceForms.Mvc.Pipelines;
using Sitecore.ExperienceForms.Mvc.Pipelines.ExecuteSubmit;
using Sitecore.ExperienceForms.Mvc.Pipelines.GetModel;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.Mvc.Pipelines;
using System.Web;

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
        private object _formBuilderContext;
        private List<IViewModel> _fields;
        private string _buttonId;
        private Item _buttonItem;

        const string SubmitFieldId = "{7CE25CAB-EF3A-4F73-AB13-D33BDC1E4EE2}";


        public SubmitForm(Item dialogAction)
        {
            _dialogAction = dialogAction;
            Guid.TryParse(_dialogAction["FormId"], out _formId);
            _mappings = (NameValueListField)_dialogAction.Fields["Mapping"];
            _formSubmitHandler = ServiceLocator.ServiceProvider.GetService(typeof(IFormSubmitHandler)) as IFormSubmitHandler;            
        }

        public bool IsPromptDialog => false;

        public virtual IFormBuilderContext FormBuilderContext => (IFormBuilderContext)(_formBuilderContext ??
            (_formBuilderContext = ServiceLocator.ServiceProvider.GetService(typeof(IFormBuilderContext))));

        public Task Execute(DialogActionContext context)
        {

            try
            {
                LoadForm();

                FillFields(context);

                var formSubmitContext = new FormSubmitContext(_buttonId)
                {
                    SessionId = Guid.NewGuid(),
                    Fields = _fields,
                    FormId = _formId,
                };               

                ExecuteActions(formSubmitContext);
            }
            catch (Exception e)
            {
                Log.Warn("Failed to submit form", e);
            }

            return Task.CompletedTask;

        }

        private void ExecuteActions(FormSubmitContext formSubmitContext)
        {
            var buttonViewModel = new ButtonViewModel();
            buttonViewModel.InitializeModel(_buttonItem);

            using (ExecuteSubmitActionsEventArgs args = new ExecuteSubmitActionsEventArgs())
            {
                args.FormSubmitContext = formSubmitContext;
                List<ParameterizedSubmitAction> parameterizedSubmitActionList =
                    buttonViewModel.SubmitActions
                        .Select(a => CreateActionEx(a, args.FormBuilderContext))
                        .Where(s => s != null)
                        .ToList();
                args.SubmitActions =
                    parameterizedSubmitActionList;

                foreach (ParameterizedSubmitAction submitAction in args.SubmitActions)
                {
                    if (args.FormSubmitContext.Canceled)
                        break;
                    submitAction.SubmitAction.ExecuteAction(args.FormSubmitContext, submitAction.Parameters);
                }
            }
        }

        private void LoadForm()
        {
            _formItem = Sitecore.Context.Database.GetItem(ID.Parse(_formId));
            _fieldDictionary = new Dictionary<string, Item>();

            if (_formItem == null)
            {
                return;
            }

            var pages = _formItem.Children;

            foreach (Item page in pages)
            {
                if (page == null)
                {
                    continue;
                }

                foreach (Item pageChild in page.Children)
                {
                    var fieldTypeId = pageChild.Fields["Field Type"].Value;

                    if (ID.Parse(fieldTypeId) == ID.Parse(SubmitFieldId))
                    {
                        _buttonId = pageChild.ID.ToString();
                        _buttonItem = pageChild;
                        continue;
                    }

                    _fieldDictionary.Add(pageChild.Name, pageChild);
                }
            }
        }

        private void FillFields(DialogActionContext submitContext)
        {
            _fields = new List<IViewModel>();

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

                var fieldItem = _fieldDictionary[mappingsNameValue];

                var fieldTypeId = fieldItem.Fields["Field Type"].Value;

                var modelTypeItem = FindItemById(fieldTypeId);

                if (modelTypeItem != null)
                {
                    var viewModel = InstanceHelper.CreateInstance(modelTypeItem.ModelType) as FieldViewModel;                    
                    viewModel?.InitializeModel(fieldItem);

                    if (viewModel is InputViewModel<string> inputModel)
                    {
                        inputModel.Value = variableValue;
                    }

                    if (viewModel is InputViewModel<DateTime?> dateModel &&
                        DateTime.TryParse(variableValue, out var dateTimeValue))
                    {
                        dateModel.Value = dateTimeValue;
                    }

                    if (viewModel is InputViewModel<bool> boolModel)
                    {
                        bool.TryParse(variableValue, out var boolValue);

                        if (boolValue || variableValue?.ToLower().Trim() == "true")
                        {
                            boolModel.Value = true;
                        }
                        else
                        {
                            boolModel.Value = false;
                        }
                    }

                    _fields.Add(viewModel);
                }
            }
        }


        protected ParameterizedSubmitAction CreateActionEx(
            SubmitActionDefinitionData submitActionDefinitionData,
            IFormBuilderContext formBuilderContext)
        {
            return CreateSubmitAction(submitActionDefinitionData, formBuilderContext);
        }

        protected FieldTypeSearchResultItem FindItemById(string id)
        {
            var index = ContentSearchManager.GetIndex("sitecore_web_index");

            using (var context = index.CreateSearchContext())
            {
                var itemId = ID.Parse(id);

                var modelTypeItem =
                    context
                        .GetQueryable<FieldTypeSearchResultItem>()
                        .FirstOrDefault(i => i.ItemId == itemId);

                return modelTypeItem;
            }

        }

        protected ParameterizedSubmitAction CreateSubmitAction(
            SubmitActionDefinitionData submitActionDefinitionData,
            IFormBuilderContext formBuilderContext)
        {

            var submitActionItem = FindItemById(submitActionDefinitionData.SubmitActionId);

            if (submitActionItem != null)
            {
                SubmitActionData submitActionData = new SubmitActionData();
                if (InstanceHelper.CreateInstance(submitActionItem.ModelType, (object)submitActionData) is ISubmitAction instance)
                    return new ParameterizedSubmitAction()
                    {
                        SubmitAction = instance,
                        Parameters = submitActionDefinitionData.Parameters
                    };
            }
            return null;
        }
    }
}
