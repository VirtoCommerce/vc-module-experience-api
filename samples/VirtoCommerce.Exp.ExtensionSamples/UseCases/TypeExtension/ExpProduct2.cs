using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.XDigitalCatalog;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class ExpProduct2 : ExpProduct
    {
        [BindIndexField(FieldName = "__content")]
        public string[] SearchKeywords { get; set; }

        [BindIndexField(FieldName = "__inventories", BinderType = typeof(InventoryBinder))]
        public IList<Inventory> InventoriesFromIndex { get; set; } = new List<Inventory>();


    }
}
