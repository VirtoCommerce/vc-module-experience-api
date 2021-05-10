using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IMemberAggregateFactory _memberAggregateFactory;
        private readonly NewContactValidator _validator;

        public CreateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, IMemberAggregateFactory factory, NewContactValidator validator)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _memberAggregateFactory = factory;
            _validator = validator;
        }

        public async Task<ContactAggregate> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            request.MemberType = nameof(Contact);

            var contactAggregate = _memberAggregateFactory.Create<ContactAggregate>(request);

            await _validator.ValidateAndThrowAsync(contactAggregate.Contact);

            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
