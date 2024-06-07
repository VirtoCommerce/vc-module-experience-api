using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Binding
{
    public class KeyBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; }

        public virtual object BindModel(SearchDocument searchDocument)
        {
            return searchDocument.Id;
        }
    }
}
