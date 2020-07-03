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
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            var lineItem = cartAggr.Cart.Items.FirstOrDefault(x => x.Id.Equals(request.LineItemId));
            CartProduct product = null;
            if(lineItem != null)
            {
                product = (await _cartProductService.GetCartProductsByIdsAsync(cartAggr, new[] { lineItem.ProductId })).FirstOrDefault();
            }
            await cartAggr.ChangeItemQuantityAsync(new ItemQtyAdjustment(request.LineItemId, request.Quantity, product));
            await CartRepository.SaveAsync(cartAggr);
            return cartAggr;
        }
    }
}
