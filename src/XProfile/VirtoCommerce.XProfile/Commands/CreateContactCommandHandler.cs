using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;

        public CreateContactCommandHandler(IContactAggregateRepository contactAggregateRepository)
        {
            _contactAggregateRepository = contactAggregateRepository;
        }
        public async Task<ContactAggregate> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var contactAggregate = new ContactAggregate(request);

            await new NewContactValidator().ValidateAndThrowAsync(contactAggregate.Contact);

            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
