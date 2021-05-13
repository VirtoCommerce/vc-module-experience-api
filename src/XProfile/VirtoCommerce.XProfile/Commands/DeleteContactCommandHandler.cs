using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, bool>
    {
        private readonly IContactAggregateRepository _contactAggregateRepository;
        public DeleteContactCommandHandler(IContactAggregateRepository contactAggregateRepository)
        {
            _contactAggregateRepository = contactAggregateRepository;
        }
        public async Task<bool> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            await _contactAggregateRepository.DeleteAsync(request.ContactId);

            return true;
        }
    }
}
