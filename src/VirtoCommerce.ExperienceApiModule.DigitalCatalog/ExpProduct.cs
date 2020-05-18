using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Binders;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public class ExpProduct
    {
        public string Id => CatalogProduct.Id;
        [BindIndexField(FieldName = "__object", BinderType = typeof(CatalogProductBinder))]
        public CatalogProduct CatalogProduct{ get; set; }

        [BindIndexField(FieldName = "__prices", BinderType = typeof(PriceBinder))]
        public IList<Price> Prices { get; set; }

        public string DataSource { get; set; }
    }
}
