using System.Reflection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Binding
{
    public class GenericModelBinder<TResult> : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; }

        public virtual object BindModel(SearchDocument searchDocument)
        {
            var result = AbstractTypeFactory<TResult>.TryCreateInstance();
            var properties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var binder = property.GetIndexModelBinder();

                if (binder != null)
                {
                    property.SetValue(result, binder.BindModel(searchDocument));
                }
            }

            return result;
        }
    }
}
