using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Extension.Binders
{
    public class PriceBinder : IIndexModelBinder
    {
        private static readonly Regex _priceFieldRegExp = new Regex(@"^price_([A-Za-z]{3})_?([a-z0-9]+)?$", RegexOptions.Compiled);

        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__prices" };

        public object BindModel(SearchDocument doc)
        {
            var result = new List<Price>();
            if (doc.ContainsKey(BindingInfo.FieldName))
            {
                var obj = doc[BindingInfo.FieldName];
                if (obj is Array jobjArray)
                {
                    var prices = jobjArray.OfType<JObject>().Select(x => (Price)x.ToObject(typeof(Price)));
                    result.AddRange(prices);
                }
            }
            else
            {
                foreach (var pair in doc)
                {
                    var match = _priceFieldRegExp.Match(pair.Key);
                    if (match.Success)
                    {
                        foreach (var listPrice in pair.Value is Array ? (object[])pair.Value : new[] { pair.Value })
                        {
                            var price = new Price
                            {
                                Currency = match.Groups[1].Value,
                                PriceListId = match.Groups[2].Value,
                                List = Convert.ToDecimal(listPrice)
                            };
                            result.Add(price);
                        }
                    }
                }
            }
            return result;
        }
    }
}
