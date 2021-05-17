using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ChangeCartItemCommentCommandHandler : CartCommandHandler<ChangeCartItemCommentCommand>
    {
        private readonly ICartProductService _cartProductService;

        public ChangeCartItemCommentCommandHandler(ICartAggregateRepository cartRepository, ICartProductService cartProductService)
            : base(cartRepository)
        {
            _cartProductService = cartProductService;
        }

        public override async Task<CartAggregate> Handle(ChangeCartItemCommentCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);
            var lineItem = cartAggregate.Cart.Items.FirstOrDefault(x => x.Id.Equals(request.LineItemId));

            if ((await _cartProductService.GetCartProductsByIdsAsync(cartAggregate, new[] { lineItem.ProductId })).FirstOrDefault() == null)
            {
                return cartAggregate;
            }

            await cartAggregate.ChangeItemCommentAsync(new NewItemComment(request.LineItemId, request.Comment));

            return await SaveCartAsync(cartAggregate);
        }
    }
}
