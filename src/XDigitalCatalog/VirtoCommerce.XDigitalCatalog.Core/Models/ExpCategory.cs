using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.XDigitalCatalog.Core.Binding;

namespace VirtoCommerce.XDigitalCatalog.Core.Models
{
    public class ExpCategory
    {
        public string Id => Category?.Id;

        [BindIndexField(FieldName = "__object", BinderType = typeof(CategoryBinder))]
        public virtual Category Category { get; set; }

        [BindIndexField(BinderType = typeof(KeyBinder))]
        public virtual string Key { get; set; }

        //Level in hierarchy
        public int Level => Category?.Outline?.Split("/").Length ?? 0;

        public IList<ExpCategory> ChildCategories { get; set; }
    }
}
