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

        protected async virtual Task<CartAggregate> GetOrCreateCartFromCommandAsync(TCartCommand request)
        {
            var result = await CartRepository.GetCartAsync(request.CartName, request.StoreId, request.UserId, request.Language, request.Currency, request.CartType);
            if (result == null)
            {
                result = await CreateNewCartAggregateAsync(request);
            }
            return result;
        }

        protected async virtual Task<ShoppingCart> GetCartById(string cartId, string language) => (await CartRepository.GetCartByIdAsync(cartId, language))?.Cart;

        protected virtual Task<CartAggregate> CreateNewCartAggregateAsync(TCartCommand request)
        {
            var cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();

            cart.CustomerId = request.UserId;
            cart.Name = request.CartName ?? "default";
            cart.StoreId = request.StoreId;
            cart.LanguageCode = request.Language;
            cart.Type = request.CartType;
            cart.Currency = request.Currency;
            cart.Items = new List<LineItem>();
            cart.Shipments = new List<Shipment>();
            cart.Payments = new List<Payment>();
            cart.Addresses = new List<CartModule.Core.Model.Address>();
            cart.TaxDetails = new List<TaxDetail>();
            cart.Coupons = new List<string>();

            return CartRepository.GetCartForShoppingCartAsync(cart);
        }
    }
}
