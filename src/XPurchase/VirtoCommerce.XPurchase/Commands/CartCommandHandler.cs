using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class CartCommandHandler<TCartCommand> : IRequestHandler<TCartCommand, CartAggregate> where TCartCommand : CartCommand
    {
        protected CartCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            CartAggrRepository = cartAggrRepository;
        }
        protected ICartAggregateRepository CartAggrRepository { get; private set; }

        public abstract Task<CartAggregate> Handle(TCartCommand request, CancellationToken cancellationToken);

        protected Task<CartAggregate> GetCartAggregateFromCommandAsync(TCartCommand request)
        {
            return CartAggrRepository.GetOrCreateAsync(request.CartName, request.StoreId, request.UserId, request.Language, request.Currency, request.CartType);
        }
    }
}
