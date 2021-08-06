using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IMemberAggregateFactory _memberAggregateFactory;

        public UpdateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, IMemberAggregateFactory factory)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _memberAggregateFactory = factory;
        }
        public virtual async Task<ContactAggregate> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contactAggregate = _memberAggregateFactory.Create<ContactAggregate>(request);

            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
