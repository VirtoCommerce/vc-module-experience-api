using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemCommentCommandHangler : CartCommandHandler<ChangeCartItemCommentCommand>
    {
        private readonly ICartProductService _cartProductService;

        public ChangeCartItemCommentCommandHangler(ICartAggregateRepository cartRepository, ICartProductService cartProductService)
            : base(cartRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemCommentCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetOrCreateCartFromCommandAsync(request);
            var lineItem = cartAggr.Cart.Items.FirstOrDefault(x => x.Id.Equals(request.LineItemId));

            if ((await _cartProductService.GetCartProductsByIdsAsync(cartAggr, new[] { lineItem.ProductId })).FirstOrDefault() == null)
            {
                return cartAggr;
            }

            await cartAggr.ChangeItemCommentAsync(new NewItemComment(request.LineItemId, request.Comment));
            await CartRepository.SaveAsync(cartAggr);

            return cartAggr;
        }
    }
}
