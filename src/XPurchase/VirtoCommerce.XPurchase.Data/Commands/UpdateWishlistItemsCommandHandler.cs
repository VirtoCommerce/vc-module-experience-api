using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class UpdateWishlistItemsCommandHandler : CartCommandHandler<UpdateWishlistItemsCommand>
    {
        private readonly ICartProductService _cartProductService;

        public UpdateWishlistItemsCommandHandler(ICartAggregateRepository cartAggrRepository, ICartProductService cartProductService)
            : base(cartAggrRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(UpdateWishlistItemsCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            cartAggregate.ValidationRuleSet = new string[] { "default" };

            foreach (var item in request.Items)
            {
                var lineItem = cartAggregate.Cart.Items.FirstOrDefault(x => x.Id.Equals(item.LineItemId));
                if (lineItem != null)
                {
                    var product = (await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { lineItem.ProductId })).FirstOrDefault();

                    await cartAggregate.ChangeItemQuantityAsync(new ItemQtyAdjustment
                    {
                        LineItem = lineItem,
                        LineItemId = item.LineItemId,
                        NewQuantity = item.Quantity,
                        CartProduct = product
                    });
                }
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}
