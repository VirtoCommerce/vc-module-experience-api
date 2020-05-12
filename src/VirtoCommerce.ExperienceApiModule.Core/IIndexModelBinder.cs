using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    public interface IIndexModelBinder
    {
        BindingInfo BindingInfo { get; set; }
        object BindModel(SearchDocument doc);
    }
}
