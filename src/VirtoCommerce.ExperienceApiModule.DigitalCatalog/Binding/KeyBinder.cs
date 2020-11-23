using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class KeyBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "key" };

        public virtual object BindModel(SearchDocument searchDocument)
        {
            if (searchDocument.ContainsKey(BindingInfo.FieldName))
            {
                return searchDocument[BindingInfo.FieldName]?.ToString();
            }

            // No key in index
            return null;
        }
    }
}
