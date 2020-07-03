using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Binding
{
    public class InventoryBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__inventories" };

        public object BindModel(SearchDocument doc)
        {
            var result = new List<InventoryInfo>();
            if (doc.ContainsKey(BindingInfo.FieldName))
            {
                var obj = doc[BindingInfo.FieldName];
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
