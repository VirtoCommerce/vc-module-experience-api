using System.Reflection;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Binding
{
    public class ExpCategoryBinder : IIndexModelBinder
    {
        public BindingInfo BindingInfo { get; set; }

        public virtual object BindModel(SearchDocument searchDocument)
        {
            var result = AbstractTypeFactory<ExpCategory>.TryCreateInstance();
            var productProperties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in productProperties)
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
