using System.Collections.Generic;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    public interface IIndexModelBinder
    {
        object BindModel(SearchDocument doc, BindingInfo bindingInfo);
    }
}
