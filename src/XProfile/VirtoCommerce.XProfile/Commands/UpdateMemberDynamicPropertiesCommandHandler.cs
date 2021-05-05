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
        private readonly IDynamicPropertyDictionaryItemsSearchService _dynamicPropertyDictionaryItemsSearchService;


        public UpdateMemberDynamicPropertiesCommandHandler(IContactAggregateRepository contactAggregateRepository, IDynamicPropertyMetaDataResolver metadataResolver, IDynamicPropertyDictionaryItemsSearchService dynamicPropertyDictionaryItemsSearchService)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _metadataResolver = metadataResolver;
            _dynamicPropertyDictionaryItemsSearchService = dynamicPropertyDictionaryItemsSearchService;
        }

        public async Task<IMemberAggregateRoot> Handle(UpdateMemberDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var memberAggregate = await _contactAggregateRepository.GetMemberAggregateRootByIdAsync(request.MemberId);
            memberAggregate.UpdateDynamicProperties(request.DynamicProperties, _metadataResolver, _dynamicPropertyDictionaryItemsSearchService);

            await _contactAggregateRepository.SaveAsync(memberAggregate);

            return await _contactAggregateRepository.GetMemberAggregateRootByIdAsync(request.MemberId);
        }
    }
}
