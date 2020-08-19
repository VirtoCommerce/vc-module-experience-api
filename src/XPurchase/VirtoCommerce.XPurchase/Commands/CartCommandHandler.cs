using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands
{
    public abstract class CartCommandHandler<TCartCommand> : IRequestHandler<TCartCommand, CartAggregate> where TCartCommand : CartCommand
    {
        protected CartCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            CartRepository = cartAggrRepository;
        }

        protected ICartAggregateRepository CartRepository { get; private set; }

        public abstract Task<CartAggregate> Handle(TCartCommand request, CancellationToken cancellationToken);

        protected virtual async Task<CartAggregate> GetOrCreateCartFromCommandAsync(TCartCommand request)
        {
            var result = await CartRepository.GetCartAsync(request.CartName, request.StoreId, request.UserId, request.Language, request.Currency, request.CartType);
            if (result == null)
            {
                result = await CreateNewCartAggregateAsync(request);
            }
            return result;
        }

        protected virtual async Task<CartAggregate> GetCartById(string cartId, string language) => await CartRepository.GetCartByIdAsync(cartId, language);

        protected virtual Task<CartAggregate> CreateNewCartAggregateAsync(TCartCommand request)
        {
            var cart = CartRepository.CreateDefaultShoppingCart(request);

            return CartRepository.GetCartForShoppingCartAsync(cart);
        }

        protected async virtual Task<CartAggregate> SaveCartAsync(CartAggregate cartAggregate)
        {
            await CartRepository.SaveAsync(cartAggregate);
            return cartAggregate;
        }
    }
}
