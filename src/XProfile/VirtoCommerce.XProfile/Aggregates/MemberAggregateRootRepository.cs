using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Aggregates
{
    public class MemberAggregateRootRepository : IMemberAggregateRootRepository
    {
        protected readonly IMemberService _memberService;
        protected readonly IMemberAggregateFactory _memberAggregateFactory;

        public MemberAggregateRootRepository(IMemberService memberService, IMemberAggregateFactory factory)
        {
            _memberService = memberService;
            _memberAggregateFactory = factory;
        }


        public async Task<T> GetMemberAggregateRootByIdAsync<T>(string id) where T : class, IMemberAggregateRoot
        {
            var member = await _memberService.GetByIdAsync(id);
            return _memberAggregateFactory.Create<T>(member);
        }

        public Task SaveAsync(IMemberAggregateRoot aggregate)
        {
            return _memberService.SaveChangesAsync(new[] { aggregate.Member });
        }

        public Task DeleteAsync(string id)
        {
            return _memberService.DeleteAsync(new[] { id });
        }
    }
}
