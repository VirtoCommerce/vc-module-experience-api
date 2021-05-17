using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public interface IDynamicPropertyResolverService
    {
        Task<IEnumerable<DynamicPropertyObjectValue>> LoadDynamicPropertyValues(IHasDynamicProperties entity, string cultureName);
    }
}
