using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregateRepository : MemberAggregateRootRepository, IContactAggregateRepository
    {
        public ContactAggregateRepository(IMemberService memberService, MemberAggregateBuilder builder)
            : base(memberService, builder)
        {
        }
    }
}
