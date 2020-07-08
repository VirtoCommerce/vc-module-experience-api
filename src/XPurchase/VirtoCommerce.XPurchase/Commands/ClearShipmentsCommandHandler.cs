using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearShipmentsCommandHandler : IRequestHandler<ClearShipmentsCommand, bool>
    {
        public ClearShipmentsCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            CartAggrRepository = cartAggrRepository;
        }

        private ICartAggregateRepository CartAggrRepository { get; set; }

        public virtual async Task<bool> Handle(ClearShipmentsCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await CartAggrRepository.GetCartByIdAsync(request.CartId);

            if (aggregate == null)
            {
                return false;
            }

            aggregate.Cart.Shipments.Clear();

            await CartAggrRepository.SaveAsync(aggregate);

            return true;
        }
    }
}
