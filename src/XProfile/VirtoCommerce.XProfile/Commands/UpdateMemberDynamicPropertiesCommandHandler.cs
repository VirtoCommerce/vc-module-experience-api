using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateMemberDynamicPropertiesCommandHandler : IRequestHandler<UpdateMemberDynamicPropertiesCommand, IMemberAggregateRoot>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IDynamicPropertyUpdaterService _dynamicPropertyUpdater;

        public UpdateMemberDynamicPropertiesCommandHandler(IContactAggregateRepository contactAggregateRepository, IDynamicPropertyUpdaterService dynamicPropertyUpdater)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _dynamicPropertyUpdater = dynamicPropertyUpdater;
        }

        public async Task<IMemberAggregateRoot> Handle(UpdateMemberDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var memberAggregate = await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<MemberAggregateRootBase>(request.MemberId);

            await _dynamicPropertyUpdater.UpdateDynamicPropertyValues(memberAggregate.Member, request.DynamicProperties);

            await _contactAggregateRepository.SaveAsync(memberAggregate);

            return await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<MemberAggregateRootBase>(request.MemberId);
        }
    }
}
