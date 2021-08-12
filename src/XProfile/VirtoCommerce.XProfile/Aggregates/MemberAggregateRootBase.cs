using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Aggregates
{
    public abstract class MemberAggregateRootBase : IMemberAggregateRoot
    {
        public virtual Member Member { get; set; }

        public virtual MemberAggregateRootBase UpdateAddresses(IList<Address> addresses)
        {
            foreach (var address in addresses)
            {
                var addressForReplacement = Member.Addresses.FirstOrDefault(x => x.Key == address.Key);

                if (addressForReplacement != null)
                {
                    var index = Member.Addresses.IndexOf(addressForReplacement);
                    Member.Addresses[index] = address;
                }
                else
                {
                    address.Key = null;
                    Member.Addresses.Add(address);
                }
            }
            return this;
        }
    }
}
