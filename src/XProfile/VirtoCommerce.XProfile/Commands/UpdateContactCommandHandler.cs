using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly IMapper _mapper;

        public UpdateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, IMapper mapper)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _mapper = mapper;
        }
        public async Task<ContactAggregate> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contactAggregate = await _contactAggregateRepository.GetContactByIdAsync(request.Id);
            _mapper.Map(request, contactAggregate.Contact);
            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
