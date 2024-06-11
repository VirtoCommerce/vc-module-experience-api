using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Core.Commands.BaseCommands
{
    public abstract class CartCommandHandler<TCartCommand> : IRequestHandler<TCartCommand, CartAggregate> where TCartCommand : CartCommand
    {
        protected CartCommandHandler(ICartAggregateRepository cartAggregateRepository)
        {
            CartRepository = cartAggregateRepository;
        }

        protected ICartAggregateRepository CartRepository { get; private set; }

        public abstract Task<CartAggregate> Handle(TCartCommand request, CancellationToken cancellationToken);

        protected virtual async Task<CartAggregate> GetOrCreateCartFromCommandAsync(TCartCommand request)
        {
            CartAggregate result;
            if (!string.IsNullOrEmpty(request.CartId))
            {
                result = await GetCartById(request.CartId, request.CultureName);
            }
            else
            {
                var cartSearchCriteria = GetCartSearchCriteria(request);
                result = await CartRepository.GetCartAsync(cartSearchCriteria, request.CultureName);
                if (result == null)
                {
                    result = await CreateNewCartAggregateAsync(request);
                }
            }
            return result;
        }

        protected virtual ShoppingCartSearchCriteria GetCartSearchCriteria(TCartCommand request)
        {
            var cartSearchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();

            cartSearchCriteria.Name = request.CartName;
            cartSearchCriteria.StoreId = request.StoreId;
            cartSearchCriteria.CustomerId = request.UserId;
            cartSearchCriteria.Currency = request.CurrencyCode;
            cartSearchCriteria.Type = request.CartType;

            return cartSearchCriteria;
        }

        protected virtual Task<CartAggregate> GetCartById(string cartId, string language) => CartRepository.GetCartByIdAsync(cartId, language);

        protected virtual Task<CartAggregate> CreateNewCartAggregateAsync(TCartCommand request)
        {
            var cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();

            cart.CustomerId = request.UserId;
            cart.Name = request.CartName ?? "default";
            cart.StoreId = request.StoreId;
            cart.LanguageCode = request.CultureName;
            cart.Type = request.CartType;
            cart.Currency = request.CurrencyCode;
            cart.Items = new List<LineItem>();
            cart.Shipments = new List<Shipment>();
            cart.Payments = new List<Payment>();
            cart.Addresses = new List<CartModule.Core.Model.Address>();
            cart.TaxDetails = new List<TaxDetail>();
            cart.Coupons = new List<string>();
            cart.Discounts = new List<Discount>();
            cart.DynamicProperties = new List<DynamicObjectProperty>();

            return CartRepository.GetCartForShoppingCartAsync(cart);
        }

        protected virtual async Task<CartAggregate> SaveCartAsync(CartAggregate cartAggregate)
        {
            await CartRepository.SaveAsync(cartAggregate);
            return cartAggregate;
        }
    }
}
