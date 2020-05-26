using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class ExpProduct2 : ExpProduct
    {
        [BindIndexField(FieldName = "__inventories", BinderType = typeof(InventoryBinder))]
        public IList<Inventory> Inventories { get; set; }
    }
}
