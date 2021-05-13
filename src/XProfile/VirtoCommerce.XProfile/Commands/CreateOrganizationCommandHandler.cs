using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationAggregate>
    {
        protected readonly IMapper _mapper;
        protected readonly IOrganizationAggregateRepository _organizationAggregateRepository;
        protected readonly IMemberAggregateFactory _memberAggregateFactory;

        public CreateOrganizationCommandHandler(IMapper mapper, IOrganizationAggregateRepository organizationAggregateRepository, IMemberAggregateFactory factory)
        {
            _mapper = mapper;
            _organizationAggregateRepository = organizationAggregateRepository;
            _memberAggregateFactory = factory;
        }

        public async Task<OrganizationAggregate> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var org = _mapper.Map<Organization>(request);
            var orgAggr = _memberAggregateFactory.Create<OrganizationAggregate>(org);
            await _organizationAggregateRepository.SaveAsync(orgAggr);

            return orgAggr;
        }
    }
}
