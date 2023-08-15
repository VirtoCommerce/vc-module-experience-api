using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
                        var jObjects = new List<JObject>();
                        foreach (var sObj in jArray.OfType<string>())
                        {
                            try
                            {
                                var jObj = JObject.Parse(sObj);
                                jObjects.Add(jObj);
                            }
                            catch (JsonReaderException)
                            {
                                // do nothing;
                            }
                        }

                        jObjects = jObjects.Any() ? jObjects : jArray.OfType<JObject>().ToList();
                        foreach (var jObject in jObjects)
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

        private static void AddPrice(List<Price> result, Dictionary<string, object> valuePairs)
        {
            var price = new Price();

            if (valuePairs.TryGetValue(nameof(IndexedPrice.Currency), out var currency))
            {
                price.Currency = currency as string;
            }

            if (valuePairs.TryGetValue(nameof(IndexedPrice.Value), out var value))
            {
                price.List = Convert.ToDecimal(value);
            }

            result.Add(price);
        }
    }
}
