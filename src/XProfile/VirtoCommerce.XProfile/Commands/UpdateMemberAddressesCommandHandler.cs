using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateMemberAddressesCommandHandler : IRequestHandler<UpdateMemberAddressesCommand, MemberAggregateRootBase>
    {
        private readonly IMemberAggregateRootRepository _memberAggregateRepository;

        public UpdateMemberAddressesCommandHandler(IMemberAggregateRootRepository memberAggregateRepository)
        {
            _memberAggregateRepository = memberAggregateRepository;
        }

        public virtual async Task<MemberAggregateRootBase> Handle(UpdateMemberAddressesCommand request, CancellationToken cancellationToken)
        {
            var memberAggregate = await _memberAggregateRepository.GetMemberAggregateRootByIdAsync<MemberAggregateRootBase>(request.MemberId);
            memberAggregate.UpdateAddresses(request.Addresses);
            await _memberAggregateRepository.SaveAsync(memberAggregate);

            return await _memberAggregateRepository.GetMemberAggregateRootByIdAsync<MemberAggregateRootBase>(request.MemberId);
        }
    }
}
