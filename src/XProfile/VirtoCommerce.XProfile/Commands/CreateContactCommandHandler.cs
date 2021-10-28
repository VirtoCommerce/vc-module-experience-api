using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IMemberAggregateFactory _memberAggregateFactory;
        private readonly IDynamicPropertyUpdaterService _dynamicPropertyUpdater;
        private readonly IMapper _mapper;
        private readonly NewContactValidator _validator;

        public CreateContactCommandHandler(IContactAggregateRepository contactAggregateRepository,
            IMemberAggregateFactory factory,
            IDynamicPropertyUpdaterService dynamicPropertyUpdater,
            IMapper mapper,
            NewContactValidator validator)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _memberAggregateFactory = factory;
            _dynamicPropertyUpdater = dynamicPropertyUpdater;
            _mapper = mapper;
            _validator = validator;
        }

        public virtual async Task<ContactAggregate> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            request.MemberType = nameof(Contact);

            var contact = _mapper.Map<Contact>(request);

            var contactAggregate = _memberAggregateFactory.Create<ContactAggregate>(contact);

            await _validator.ValidateAndThrowAsync(contactAggregate.Contact);

            if (request.DynamicProperties != null)
            {
                await _dynamicPropertyUpdater.UpdateDynamicPropertyValues(contactAggregate.Contact, request.DynamicProperties);
            }

            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
