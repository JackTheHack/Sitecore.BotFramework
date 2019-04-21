using System.Runtime.Serialization;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace SC90.Bot.Forms.Actions.Models
{
    public class FieldTypeSearchResultItem:SearchResultItem
    {
        [IndexField("model_type_t")]
        [DataMember]
        public virtual string ModelType { get; set; }
    }
}
