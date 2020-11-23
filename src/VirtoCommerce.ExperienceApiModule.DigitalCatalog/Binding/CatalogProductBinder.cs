using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class CatalogProductBinder : IIndexModelBinder
    {
        private static readonly Type _productType = AbstractTypeFactory<CatalogProduct>.TryCreateInstance().GetType();

        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__object" };

        public virtual object BindModel(SearchDocument searchDocument)
        {
            var result = default(CatalogProduct);

            if (!searchDocument.ContainsKey(BindingInfo.FieldName))
            {
                // No object in index
                return result;
            }

            var obj = searchDocument[BindingInfo.FieldName];
            if (obj is JObject jobj)
            {
                result = (CatalogProduct)jobj.ToObject(_productType);

                var productProperties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in productProperties)
                {
                    var binder = property.GetIndexModelBinder();

                    if (binder != null)
                    {
                        property.SetValue(result, binder.BindModel(searchDocument));
                    }
                }
            }

            return result;
        }
    }
}
