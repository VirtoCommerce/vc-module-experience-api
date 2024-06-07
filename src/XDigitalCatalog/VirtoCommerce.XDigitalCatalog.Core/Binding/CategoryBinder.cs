using System;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Binding
{
    public class CategoryBinder : IIndexModelBinder
    {
        private static readonly Type _productType = AbstractTypeFactory<Category>.TryCreateInstance().GetType();

        public BindingInfo BindingInfo { get; set; } = new BindingInfo { FieldName = "__object" };

        public virtual object BindModel(SearchDocument searchDocument)
        {
            var fieldName = BindingInfo.FieldName;

            if (searchDocument.ContainsKey(BindingInfo.FieldName) && searchDocument[fieldName] is JObject jobj)
            {
                return (Category)jobj.ToObject(_productType);
            }

            // No object in index
            return null;
        }
    }
}
