using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    [Obsolete("We dont need this binder because we dont get __inventories from index")]
    public class InventoryBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__inventories" };

        public object BindModel(SearchDocument searchDocument)
        {
            var result = new List<InventoryInfo>();
            if (searchDocument.ContainsKey(BindingInfo.FieldName))
            {
                var obj = searchDocument[BindingInfo.FieldName];
                if (obj is Array jobjArray)
                {
                    var inventories = jobjArray.OfType<JObject>().Select(x => (InventoryInfo)x.ToObject(typeof(InventoryInfo)));
                    result.AddRange(inventories);
                }
            }

            return result;
        }
    }
}
