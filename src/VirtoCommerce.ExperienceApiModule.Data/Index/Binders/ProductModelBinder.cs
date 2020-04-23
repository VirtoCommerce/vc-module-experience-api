using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Data.Index.Binders
{
    public class ProductModelBinder : IIndexModelBinder
    {
        private readonly Type _productType = AbstractTypeFactory<CatalogProduct>.TryCreateInstance().GetType();
        public object BindModel(SearchDocument doc, BindingInfo bindingInfo)
        {
            var result = default(CatalogProduct);

            if (doc.ContainsKey(bindingInfo.FieldName))
            {
                var obj = doc[bindingInfo.FieldName];

                if (obj is JObject jobj)
                {
                    result = (CatalogProduct)jobj.ToObject(_productType);

                    var productProperties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in productProperties)
                    {
                        var binder = property.GetIndexModelBinder();

                        if (binder != null)
                        {                          
                            property.SetValue(result, binder.BindModel(doc, property.GetBindingInfo()));
                        }
                    }
                }
            }
            return result;
        }
    }
}
