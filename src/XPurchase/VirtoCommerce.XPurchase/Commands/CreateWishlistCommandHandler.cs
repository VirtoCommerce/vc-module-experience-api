using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommandHandler : IRequestHandler<CreateWishlistCommand, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;

        public CreateWishlistCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartAggregateRepository = cartAggrRepository;
        }

        public async Task<CartAggregate> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
        {
            var cart = AbstractTypeFactory<ShoppingCart>.TryCreateInstance();

            cart.CustomerId = request.UserId;
            cart.Name = request.ListName;
            cart.StoreId = request.StoreId;
            cart.LanguageCode = request.CultureName;
            cart.Type = XPurchaseConstants.ListTypeName;
            cart.Currency = request.CurrencyCode;
            cart.Items = new List<LineItem>();
            //cart.Shipments = new List<Shipment>();
            //cart.Payments = new List<Payment>();
            //cart.Addresses = new List<Address>();
            //cart.TaxDetails = new List<TaxDetail>();
            //cart.Coupons = new List<string>();

            var cartAggregate = await _cartAggregateRepository.GetCartForShoppingCartAsync(cart);
            await _cartAggregateRepository.SaveAsync(cartAggregate);

            return cartAggregate;
        }
    }
}
