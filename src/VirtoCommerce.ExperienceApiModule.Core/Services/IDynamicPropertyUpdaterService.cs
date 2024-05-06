using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public interface IDynamicPropertyUpdaterService
    {
        Task UpdateDynamicPropertyValues(IHasDynamicProperties entity, IList<DynamicPropertyValue> values);
    }
}
