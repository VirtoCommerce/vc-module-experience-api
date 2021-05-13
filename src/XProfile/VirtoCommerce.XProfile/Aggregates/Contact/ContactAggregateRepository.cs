using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile
{
    public class ContactAggregateRepository : MemberAggregateRootRepository, IContactAggregateRepository
    {
        public ContactAggregateRepository(IMemberService memberService, IMemberAggregateFactory factory)
            : base(memberService, factory)
        {
        }
    }
}
