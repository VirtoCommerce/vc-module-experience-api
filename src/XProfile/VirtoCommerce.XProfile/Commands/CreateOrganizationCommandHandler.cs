using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationAggregate>
    {
        protected readonly IMapper _mapper;
        protected readonly IOrganizationAggregateRepository _organizationAggregateRepository;
        protected readonly IMemberAggregateFactory _memberAggregateFactory;
        protected readonly IDynamicPropertyUpdaterService _dynamicPropertyUpdater;

        public CreateOrganizationCommandHandler(IMapper mapper,
            IOrganizationAggregateRepository organizationAggregateRepository,
            IMemberAggregateFactory factory,
            IDynamicPropertyUpdaterService dynamicPropertyUpdater)
        {
            _mapper = mapper;
            _organizationAggregateRepository = organizationAggregateRepository;
            _memberAggregateFactory = factory;
            _dynamicPropertyUpdater = dynamicPropertyUpdater;
        }

        public virtual async Task<OrganizationAggregate> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var org = _mapper.Map<Organization>(request);
            var orgAggr = _memberAggregateFactory.Create<OrganizationAggregate>(org);

            if (request.DynamicProperties != null)
            {
                await _dynamicPropertyUpdater.UpdateDynamicPropertyValues(orgAggr.Organization, request.DynamicProperties);
            }

            await _organizationAggregateRepository.SaveAsync(orgAggr);

            return orgAggr;
        }
    }
}
