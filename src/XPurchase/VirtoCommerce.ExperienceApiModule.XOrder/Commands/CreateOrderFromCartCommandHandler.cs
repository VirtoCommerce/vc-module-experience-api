using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using MediatR;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Commands
{
    public class CreateOrderFromCartCommandHandler : IRequestHandler<CreateOrderFromCartCommand, CustomerOrderAggregate>
    {
        private readonly IShoppingCartService _cartService;
        private readonly ICustomerOrderAggregateRepository _customerOrderAggregateRepository;
        private readonly ICartAggregateRepository _cartRepository;
        private readonly ICartValidationContextFactory _cartValidationContextFactory;

        public string ValidationRuleSet { get; set; } = "*";

        public CreateOrderFromCartCommandHandler(
            IShoppingCartService cartService,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            ICartAggregateRepository cartRepository,
            ICartValidationContextFactory cartValidationContextFactory)
        {
            _cartService = cartService;
            _customerOrderAggregateRepository = customerOrderAggregateRepository;
            _cartRepository = cartRepository;
            _cartValidationContextFactory = cartValidationContextFactory;
        }

        public virtual async Task<CustomerOrderAggregate> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetByIdAsync(request.CartId);
            var cartAggregate = await _cartRepository.GetCartForShoppingCartAsync(cart);

            // remove unselected gifts before order create
            var unselectedGifts = cartAggregate.GiftItems.Where(x => !x.SelectedForCheckout).ToList();
            if (unselectedGifts.Count != 0)
            {
                unselectedGifts.ForEach(x => cartAggregate.Cart.Items.Remove(x));
            }

            cartAggregate.Cart.OrganizationName = null;
            await cartAggregate.UpdateOrganizationName();

            await ValidateCart(cartAggregate);

            // need to check for unsaved gift items before creating an order and resave the cart, otherwise an exception will be thrown on order create
            var hasUnsavedGifts = cartAggregate.GiftItems.Any(x => x.Id == null);
            if (hasUnsavedGifts)
            {
                await _cartRepository.SaveAsync(cartAggregate);
            }

            var result = await _customerOrderAggregateRepository.CreateOrderFromCart(cartAggregate.Cart);

            // remove selected items after order create
            var selectedLineItemIds = cartAggregate.SelectedLineItems.Select(x => x.Id).ToArray();
            await cartAggregate.RemoveItemsAsync(selectedLineItemIds);

            // clear cart
            cartAggregate.Cart.Shipments?.Clear();
            cartAggregate.Cart.Payments?.Clear();
            cartAggregate.Cart.Coupons?.Clear();

            cartAggregate.Cart.PurchaseOrderNumber = string.Empty;
            cartAggregate.Cart.Comment = string.Empty;
            cartAggregate.Cart.Coupon = string.Empty;
            cartAggregate.Cart.DynamicProperties?.Clear();

            await _cartRepository.SaveAsync(cartAggregate);

            // Remark: There is potential issue, because there is no transaction thru two actions above. If a cart deletion fails, the order remains. That causes data inconsistency.
            // Unfortunately, current architecture does not allow us to support such scenarios in a transactional manner.
            return result;
        }

        protected virtual async Task ValidateCart(CartAggregate cartAggregate)
        {
            var context = await _cartValidationContextFactory.CreateValidationContextAsync(cartAggregate);
            await cartAggregate.ValidateAsync(context, ValidationRuleSet);

            var errors = cartAggregate.ValidationErrors;
            if (errors.Any())
            {
                var dictionary = errors.GroupBy(x => x.ErrorCode).ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).FirstOrDefault());
                throw new ExecutionError("The cart has validation errors", dictionary) { Code = Constants.ValidationErrorCode };
            }
        }

        [Obsolete("Use ValidateCart(CartAggregate cartAggregate)()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        protected virtual async Task ValidateCart(ShoppingCart cart)
        {
            var cartAggregate = await _cartRepository.GetCartForShoppingCartAsync(cart);
            await ValidateCart(cartAggregate);
        }
    }
}
