using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Aggregates
{
    public interface IMemberAggregateRoot
    {
        Member Member { get; set; }

        MemberAggregateRootBase UpdateAddresses(IList<Address> addresses);
    }
}
