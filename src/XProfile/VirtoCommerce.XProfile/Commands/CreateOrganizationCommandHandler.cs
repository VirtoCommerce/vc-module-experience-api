using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

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
            var organizationAggregate = _mapper.Map<OrganizationAggregate>(request);
            await _organizationAggregateRepository.SaveAsync(organizationAggregate);

            return organizationAggregate;
        }
    }
}
