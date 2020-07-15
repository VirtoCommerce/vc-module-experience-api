using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Binding
{
    public interface IIndexModelBinder
    {
        BindingInfo BindingInfo { get; set; }

        object BindModel(SearchDocument doc);
    }

    public interface IIndexModelBinder<out T>
    {
        BindingInfo BindingInfo { get; set; }

        T BindModel(SearchDocument doc);
    }
}
