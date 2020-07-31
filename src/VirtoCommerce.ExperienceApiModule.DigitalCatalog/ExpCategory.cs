using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.XDigitalCatalog.Binding;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpCategory
    {
        public string Id => Category.Id;

        [BindIndexField(FieldName = "__object", BinderType = typeof(CategoryBinder))]
        public virtual Category Category { get; set; }
    }
}
