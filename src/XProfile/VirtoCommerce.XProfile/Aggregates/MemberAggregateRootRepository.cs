using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Aggregates
{
    public class MemberAggregateRootRepository : IMemberAggregateRootRepository
    {
        protected readonly IMemberService _memberService;
        protected readonly MemberAggregateBuilder _builder;

        public MemberAggregateRootRepository(IMemberService memberService, MemberAggregateBuilder builder)
        {
            _memberService = memberService;
            _builder = builder;
        }


        public async Task<IMemberAggregateRoot> GetMemberAggregateRootByIdAsync(string id)
        {
            var member = await _memberService.GetByIdAsync(id);
            return _builder.BuildMemberAggregate(member);
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
