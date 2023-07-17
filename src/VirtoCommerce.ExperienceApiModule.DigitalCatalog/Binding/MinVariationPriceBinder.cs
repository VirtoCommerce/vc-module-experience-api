using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class MinVariationPriceBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__minvariationprice" };

        public object BindModel(SearchDocument searchDocument)
        {
            var result = new List<Price>();

            if (!searchDocument.ContainsKey(BindingInfo.FieldName))
            {
                return result;
            }

            var pricesDocumentRecord = searchDocument[BindingInfo.FieldName];
            if (pricesDocumentRecord is Array array)
            {
                result = array
                    .OfType<JObject>()
                    .Select(x => (IndexedPrice)x.ToObject(typeof(IndexedPrice)))
                    .Select(x => new Price
                    {
                        Currency = x.Currency,
                        List = x.Value
                    })
                    .ToList();
            }

            return result;
        }

        private sealed class IndexedPrice
        {
            public string Currency { get; set; }
            public decimal Value { get; set; }
        }
    }
}
