using System.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class VariationsBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__variations" };

        public virtual object BindModel(SearchDocument doc)
        {
            var fieldName = BindingInfo.FieldName;

            return doc.ContainsKey(fieldName) && doc[fieldName] is object[] objs
                ? objs.Select(x => (string)x).ToList()
                : Enumerable.Empty<string>().ToList();
        }
    }
}
