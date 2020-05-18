using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Binders
{
    public class CatalogProductBinder : IIndexModelBinder
    {
        private readonly Type _productType = AbstractTypeFactory<CatalogProduct>.TryCreateInstance().GetType();

        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__object" };

        public object BindModel(SearchDocument doc)
        {
            var result = default(CatalogProduct);

            var fieldName = BindingInfo.FieldName;
            if (doc.ContainsKey(fieldName))
            {
                var obj = doc[fieldName];

                if (obj is JObject jobj)
                {
                    result = (CatalogProduct)jobj.ToObject(_productType);

                    var productProperties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in productProperties)
                    {
                        var binder = property.GetIndexModelBinder();

                        if (binder != null)
                        {                          
                            property.SetValue(result, binder.BindModel(doc));
                        }
                    }
                }
            }
            return result;
        }

    }
}
