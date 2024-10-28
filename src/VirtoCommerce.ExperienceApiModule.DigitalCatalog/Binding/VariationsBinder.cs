using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class VariationsBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__variations" };

        public virtual object BindModel(SearchDocument searchDocument)
        {
            if (!searchDocument.TryGetValue(BindingInfo.FieldName, out var value))
            {
                return new List<string>();
            }

            // It is important to note that not all search engines, such as Lucene, support field types as collections,
            // so they may not return an array for single value
            return value switch
            {
                IEnumerable<object> objs => objs.Select(x => x.ToString()).ToList(),
                string s => [s],
                _ => []
            };
        }
    }
}
