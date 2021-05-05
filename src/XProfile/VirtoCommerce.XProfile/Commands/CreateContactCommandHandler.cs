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
        private readonly MemberAggregateBuilder _builder;


        public CreateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, MemberAggregateBuilder builder)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _builder = builder;
        }
        public async Task<ContactAggregate> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            request.MemberType = nameof(Contact);

            var contactAggregate = (ContactAggregate) _builder.BuildMemberAggregate(request);

            await new NewContactValidator().ValidateAndThrowAsync(contactAggregate.Contact);

            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
