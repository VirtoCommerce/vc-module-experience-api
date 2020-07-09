using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemQuantityCommandHangler : CartCommandHandler<ChangeCartItemQuantityCommand>
    {
        private readonly ICartProductService _cartProductService;

        public ChangeCartItemQuantityCommandHangler(ICartAggregateRepository cartRepository, ICartProductService cartProductService)
            : base(cartRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            var lineItem = cartAggregate.Cart.Items.FirstOrDefault(x => x.Id.Equals(request.LineItemId));
            CartProduct product = null;
            if (lineItem != null)
            {
                product = (await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { lineItem.ProductId })).FirstOrDefault();
            }
            await cartAggregate.ChangeItemQuantityAsync(new ItemQtyAdjustment(request.LineItemId, request.Quantity, product));

            return await SaveCartAsync(cartAggregate);
        }
    }
}
