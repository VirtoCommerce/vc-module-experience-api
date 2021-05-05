using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactAggregate>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        private readonly MemberAggregateBuilder _builder;

        public UpdateContactCommandHandler(IContactAggregateRepository contactAggregateRepository, MemberAggregateBuilder builder)
        {
            _contactAggregateRepository = contactAggregateRepository;
            _builder = builder;
        }
        public async Task<ContactAggregate> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var contactAggregate = (ContactAggregate) _builder.BuildMemberAggregate(request);

            await _contactAggregateRepository.SaveAsync(contactAggregate);

            return contactAggregate;
        }
    }
}
