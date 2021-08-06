using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateContactAddressesCommandHandler : IRequestHandler<UpdateContactAddressesCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;

        public UpdateContactAddressesCommandHandler(IContactAggregateRepository contactAggregateRepository)
        {
            _contactAggregateRepository = contactAggregateRepository;
        }

        public virtual async Task<ContactAggregate> Handle(UpdateContactAddressesCommand request, CancellationToken cancellationToken)
        {
            var contactAggregate = await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<ContactAggregate>(request.ContactId);
            contactAggregate.UpdateAddresses(request.Addresses);
            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return await _contactAggregateRepository.GetMemberAggregateRootByIdAsync<ContactAggregate>(request.ContactId);
        }
    }
}
