using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.ExperienceApiModule.XProfile.Schemas;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateMemberDynamicPropertiesCommand : ICommand<IMemberAggregateRoot>
    {
        public string MemberId { get; set; }
        public IList<DynamicPropertyValue> DynamicProperties { get; set; }

        public UpdateMemberDynamicPropertiesCommand(string memberId, IList<DynamicPropertyValue> dynamicProperties)
        {
            MemberId = memberId;
            DynamicProperties = dynamicProperties;
        }
    }
}
