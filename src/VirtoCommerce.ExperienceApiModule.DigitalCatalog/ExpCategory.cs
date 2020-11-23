using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
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

        /// <summary>
        /// Request related catalog the  parent categories ids concatenated with "/". E.g. (1/21/344)
        /// </summary>
        public string Outline { get; set; }

        //Level in hierarchy
        public int Level => Category?.Outline?.Split("/").Count() ?? 0;

        /// <summary>
        /// Request related slug  path e.g /camcorders
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Request related parent category Id
        /// </summary>
        public string ParentId { get; set; }

        public SeoInfo SeoInfo { get; set; }
    }
}
