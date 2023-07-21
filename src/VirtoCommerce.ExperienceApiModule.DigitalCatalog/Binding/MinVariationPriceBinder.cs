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
                        foreach (var jObject in jArray.OfType<JObject>())
                        {
                            AddPrice(result, jObject);
                        }

                        break;
                    }

                case JObject jObject:
                    {
                        AddPrice(result, jObject);
                        break;
                    }
            }

            return result;
        }

        private static void AddPrice(List<Price> result, JObject jObject)
        {
            var indexedPrice = jObject.ToObject<IndexedPrice>();
            result.Add(new Price
            {
                Currency = indexedPrice.Currency,
                List = indexedPrice.Value,
            });
        }
    }
}
