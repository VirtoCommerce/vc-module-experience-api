using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;
        private readonly IMapper _mapper;
        private readonly IDynamicPropertyUpdaterService _dynamicPropertyUpdater;

        public UpdateOrganizationCommandHandler(IMapper mapper,
            IOrganizationAggregateRepository organizationAggregateRepository,
            IDynamicPropertyUpdaterService dynamicPropertyUpdater)
        {
            _mapper = mapper;
            _organizationAggregateRepository = organizationAggregateRepository;
            _dynamicPropertyUpdater = dynamicPropertyUpdater;
        }

        public virtual async Task<OrganizationAggregate> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organizationAggregate = await _organizationAggregateRepository.GetMemberAggregateRootByIdAsync<OrganizationAggregate>(request.Id);
            _mapper.Map(request, organizationAggregate.Organization);

            if (request.DynamicProperties != null)
            {
                await _dynamicPropertyUpdater.UpdateDynamicPropertyValues(organizationAggregate.Organization, request.DynamicProperties);
            }

            await _organizationAggregateRepository.SaveAsync(organizationAggregate);

            return organizationAggregate;
        }
    }
}
