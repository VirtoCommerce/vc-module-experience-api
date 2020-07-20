using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationAggregate>
    {
        private readonly IMapper _mapper;
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public CreateOrganizationCommandHandler(IMapper mapper, IOrganizationAggregateRepository organizationAggregateRepository)
        {
            _mapper = mapper;
            _organizationAggregateRepository = organizationAggregateRepository;
        }

        public async Task<OrganizationAggregate> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var org = _mapper.Map<Organization>(request);
            var orgAggr = new OrganizationAggregate(org);
            await _organizationAggregateRepository.SaveAsync(orgAggr);

            return orgAggr;
        }
    }
}
