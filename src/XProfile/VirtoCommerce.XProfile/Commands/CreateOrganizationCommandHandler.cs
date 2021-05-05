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
        protected readonly MemberAggregateBuilder _builder;

        public CreateOrganizationCommandHandler(IMapper mapper, IOrganizationAggregateRepository organizationAggregateRepository, MemberAggregateBuilder builder)
        {
            _mapper = mapper;
            _organizationAggregateRepository = organizationAggregateRepository;
            _builder = builder;
        }

        public async Task<OrganizationAggregate> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var org = _mapper.Map<Organization>(request);
            var orgAggr = (OrganizationAggregate)_builder.BuildMemberAggregate(org);
            await _organizationAggregateRepository.SaveAsync(orgAggr);

            return orgAggr;
        }
    }
}
