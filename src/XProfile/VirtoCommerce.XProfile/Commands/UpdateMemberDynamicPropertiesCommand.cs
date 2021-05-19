using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateMemberDynamicPropertiesCommand : ICommand<IMemberAggregateRoot>
    {
        public string MemberId { get; set; }
        public IList<DynamicPropertyValue> DynamicProperties { get; set; }
    }
}
