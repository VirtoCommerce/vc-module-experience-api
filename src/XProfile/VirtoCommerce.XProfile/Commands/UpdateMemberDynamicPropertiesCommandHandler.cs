using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateMemberDynamicPropertiesCommandHandler : IRequestHandler<UpdateMemberDynamicPropertiesCommand, IMemberAggregateRoot>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IDynamicPropertyMetaDataResolver _metadataResolver;


        public UpdateMemberDynamicPropertiesCommandHandler(IContactAggregateRepository contactAggregateRepository, IDynamicPropertyMetaDataResolver metadataResolver)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _metadataResolver = metadataResolver;
        }

        public async Task<IMemberAggregateRoot> Handle(UpdateMemberDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var memberAggregate = await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<MemberAggregateRootBase>(request.MemberId);

            await memberAggregate.UpdateDynamicPropertiesAsync(request.DynamicProperties, _metadataResolver);

            await _contactAggregateRepository.SaveAsync(memberAggregate);

            return await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<MemberAggregateRootBase>(request.MemberId);
        }
    }
}
