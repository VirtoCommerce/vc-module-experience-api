using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Data.Index.Binders;
using VirtoCommerce.ExperienceApiModule.Extension.Binders;

namespace VirtoCommerce.ExperienceApiModule.Extension
{
    [BindIndexField(FieldName = "__object", BinderType = typeof(ProductModelBinder))]
    public class CatalogProduct2 : CatalogProduct
    {
        [BindIndexField(FieldName = "__prices", BinderType = typeof(PriceBinder))]
        public IList<Price> Prices { get; set; }

        public string DataSource { get; set; }
    }
}
