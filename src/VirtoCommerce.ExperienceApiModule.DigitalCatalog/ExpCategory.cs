using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Binding;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpCategory
    {
        public string Id => Category?.Id;

        [BindIndexField(FieldName = "__object", BinderType = typeof(CategoryBinder))]
        public virtual Category Category { get; set; }

        [BindIndexField(BinderType = typeof(KeyBinder))]
        public virtual string Key { get; set; }

        //Level in hierarchy
        public int Level => Category?.Outline?.Split("/").Count() ?? 0;
    }
}
