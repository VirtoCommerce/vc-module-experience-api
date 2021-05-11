using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;
        private readonly IMapper _mapper;

        public UpdateOrganizationCommandHandler(IMapper mapper, IOrganizationAggregateRepository organizationAggregateRepository)
        {
            _mapper = mapper;
            _organizationAggregateRepository = organizationAggregateRepository;
        }

        public async Task<OrganizationAggregate> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organizationAggregate = await _organizationAggregateRepository.GetMemberAggregateRootByIdAsync<OrganizationAggregate>(request.Id);
            _mapper.Map(request, organizationAggregate.Organization);
            await _organizationAggregateRepository.SaveAsync(organizationAggregate);

            return organizationAggregate;
        }
    }
}
