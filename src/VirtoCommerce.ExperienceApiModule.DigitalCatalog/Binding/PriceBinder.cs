using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class PriceBinder : IIndexModelBinder
    {
        private static readonly Regex _priceFieldRegExp = new Regex(@"^price_([A-Za-z]{3})_?([a-z0-9]+)?$", RegexOptions.Compiled);

        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__prices" };

        public virtual object BindModel(SearchDocument searchDocument)
        {
            var result = new List<Price>();
            if (searchDocument.ContainsKey(BindingInfo.FieldName))
            {
                var obj = searchDocument[BindingInfo.FieldName];
                if (obj is Array jobjArray)
                {
                    var prices = jobjArray.OfType<JObject>().Select(x => (Price)x.ToObject(typeof(Price)));
                    result.AddRange(prices);
                }
            }
            else
            {
                foreach (var pair in searchDocument)
                {
                    var match = _priceFieldRegExp.Match(pair.Key);
                    if (!match.Success) continue;

                    foreach (var listPrice in pair.Value is Array ? (object[])pair.Value : new[] { pair.Value })
                    {
                        var list = Convert.ToDecimal(listPrice);
                        if (list == default) continue;

                        var price = new Price
                        {
                            Currency = match.Groups[1].Value.ToUpperInvariant(),
                            PricelistId = match.Groups[2].Value,
                            List = list
                        };

                        result.Add(price);
                    }
                }
            }
            return result;
        }
    }
}
