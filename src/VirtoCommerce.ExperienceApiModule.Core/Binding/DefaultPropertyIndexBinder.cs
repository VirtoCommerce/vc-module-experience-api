using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Binding
{
    public class DefaultPropertyIndexBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; }

        public object BindModel(SearchDocument searchDocument)
        {
            var fieldName = BindingInfo?.FieldName;
            if (!string.IsNullOrEmpty(fieldName) && searchDocument.ContainsKey(fieldName))
            {
                return searchDocument[fieldName];
            }
            return null;
        }
    }
}
