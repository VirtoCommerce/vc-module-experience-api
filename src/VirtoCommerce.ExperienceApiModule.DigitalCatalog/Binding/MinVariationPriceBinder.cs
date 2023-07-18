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
            switch (pricesDocumentRecord)
            {
                case Array jArray:
                    {
                        result = jArray
                            .OfType<JObject>()
                            .Select(x => x.ToObject<IndexedPrice>())
                            .Select(x => new Price
                            {
                                Currency = x.Currency,
                                List = x.Value,
                            })
                            .ToList();
                        break;
                    }

                case JObject jObject:
                    {
                        var indexedPrice = jObject.ToObject<IndexedPrice>();
                        result.Add(new Price
                        {
                            Currency = indexedPrice.Currency,
                            List = indexedPrice.Value,
                        });
                        break;
                    }
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
