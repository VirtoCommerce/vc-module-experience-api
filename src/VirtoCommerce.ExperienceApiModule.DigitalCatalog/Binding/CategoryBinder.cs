using System;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class CategoryBinder : IIndexModelBinder
    {
        private static readonly Type _productType = AbstractTypeFactory<Category>.TryCreateInstance().GetType();

        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__object" };

        public virtual object BindModel(SearchDocument searchDocument)
        {
            var fieldName = BindingInfo.FieldName;

            if (!searchDocument.ContainsKey(fieldName))
            {
                throw new InvalidOperationException($"{BindingInfo.FieldName} is missed in index data. Unable to load Category object from index.");
            }

            if (!(searchDocument[fieldName] is JObject jobj))
            {
                return null;
            }

            var result = (Category)jobj.ToObject(_productType);
            return result;
        }
    }
}
