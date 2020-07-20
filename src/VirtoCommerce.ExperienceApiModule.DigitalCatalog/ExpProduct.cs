using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Binding;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpProduct
    {
        public string Id => CatalogProduct.Id;

        [BindIndexField(FieldName = "__object", BinderType = typeof(CatalogProductBinder))]
        public virtual CatalogProduct CatalogProduct { get; set; }

        [BindIndexField(FieldName = "__variations", BinderType = typeof(VariationsBinder))]
        public virtual IList<string> VariationIds { get; set; }

        [BindIndexField(FieldName = "__prices", BinderType = typeof(PriceBinder))]
        public virtual IList<Price> Prices { get; set; }

        [BindIndexField(FieldName = "__inventories", BinderType = typeof(InventoryBinder))]
        public IList<InventoryInfo> Inventories { get; set; }

        public string DataSource { get; set; }
    }
}
