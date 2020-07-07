using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IMapper _mapper;

        public CreateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, IMapper mapper)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _mapper = mapper;
        }
        public async Task<ContactAggregate> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var contactAggregate = new ContactAggregate(AbstractTypeFactory<Contact>.TryCreateInstance());
            _mapper.Map(request, contactAggregate.Contact);
            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
