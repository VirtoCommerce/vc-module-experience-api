using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetContactByIdQueryHandler : IQueryHandler<GetContactByIdQuery, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;

        public GetContactByIdQueryHandler(IContactAggregateRepository contactAggregateRepository)
        {
            _contactAggregateRepository = contactAggregateRepository;
        }
        public async Task<ContactAggregate> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            return (ContactAggregate) await _contactAggregateRepository.GetMemberAggregateRootByIdAsync(request.ContactId);
        }
    }
}
