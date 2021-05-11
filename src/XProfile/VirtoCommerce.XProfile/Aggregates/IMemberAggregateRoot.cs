using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Aggregates
{
    public interface IMemberAggregateRoot
    {
        Member Member { get; set; }

        MemberAggregateRootBase UpdateAddresses(IList<Address> addresses);
        Task<MemberAggregateRootBase> UpdateDynamicPropertiesAsync(IList<DynamicPropertyValue> values, IDynamicPropertyMetaDataResolver metaDataResolver);
    }
}
