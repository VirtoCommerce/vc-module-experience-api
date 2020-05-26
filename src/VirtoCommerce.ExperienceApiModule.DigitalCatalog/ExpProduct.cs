using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Binding;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public class ExpProduct
    {
        public string Id => CatalogProduct.Id;
        [BindIndexField(FieldName = "__object", BinderType = typeof(CatalogProductBinder))]
        public virtual CatalogProduct CatalogProduct { get; set; }

        [BindIndexField(FieldName = "__prices", BinderType = typeof(PriceBinder))]
        public virtual IList<Price> Prices { get; set; }

        public string DataSource { get; set; }
    }
}
