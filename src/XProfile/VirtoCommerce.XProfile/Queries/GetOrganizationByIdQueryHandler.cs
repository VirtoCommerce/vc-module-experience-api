using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetOrganizationByIdQueryHandler : IQueryHandler<GetOrganizationByIdQuery, OrganizationAggregate>
    {
        private readonly IOrganizationAggregateRepository _organizationAggregateRepository;

        public GetOrganizationByIdQueryHandler(IOrganizationAggregateRepository organizationAggregateRepository)
        {
            _organizationAggregateRepository = organizationAggregateRepository;
        }

        public async Task<OrganizationAggregate> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
        {
            return (OrganizationAggregate)await _organizationAggregateRepository.GetMemberAggregateRootByIdAsync(request.OrganizationId);
        }
    }
}
