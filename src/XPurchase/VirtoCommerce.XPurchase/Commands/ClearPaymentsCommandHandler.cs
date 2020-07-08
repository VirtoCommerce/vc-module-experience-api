using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ClearPaymentsCommandHandler : IRequestHandler<ClearPaymentsCommand, bool>
    {
        public ClearPaymentsCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            CartAggrRepository = cartAggrRepository;
        }

        private ICartAggregateRepository CartAggrRepository { get; set; }

        public virtual async Task<bool> Handle(ClearPaymentsCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await CartAggrRepository.GetCartByIdAsync(request.CartId);

            if (aggregate == null)
            {
                return false;
            }

            aggregate.Cart.Payments.Clear();

            await CartAggrRepository.SaveAsync(aggregate);

            return true;
        }
    }
}
