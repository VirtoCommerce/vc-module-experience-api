using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RemoveCartCommandHandler : IRequestHandler<RemoveCartCommand, bool>
    {
        public RemoveCartCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            CartAggrRepository = cartAggrRepository;
        }

        private ICartAggregateRepository CartAggrRepository { get; set; }

        public virtual async Task<bool> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            await CartAggrRepository.RemoveCartAsync(request.CartId);

            return true;
        }
    }
}
