using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateWishlistItemCommandHandler : CartCommandHandler<UpdateWishlistItemCommand>
    {
        private readonly ICartProductService _cartProductService;

        public UpdateWishlistItemCommandHandler(ICartAggregateRepository cartAggrRepository, ICartProductService cartProductService)
            : base(cartAggrRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(UpdateWishlistItemCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            cartAggregate.ValidationRuleSet = new string[] { "default" };

            if (request.Quantity == 0)
            {
                await cartAggregate.RemoveItemAsync(request.LineItemId);
                return await SaveCartAsync(cartAggregate);
            }

            var lineItem = cartAggregate.Cart.Items.FirstOrDefault(x => x.Id.Equals(request.LineItemId));
            if (lineItem != null)
            {
                var product = (await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { lineItem.ProductId })).FirstOrDefault();

                await cartAggregate.ChangeItemQuantityAsync(new ItemQtyAdjustment
                {
                    LineItem = lineItem,
                    LineItemId = request.LineItemId,
                    NewQuantity = request.Quantity,
                    CartProduct = product
                });
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}
