using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class RenameWishlistCommandHandler : CartCommandHandler<RenameWishlistCommand>
    {
        public RenameWishlistCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(RenameWishlistCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CartRepository.GetCartByIdAsync(request.ListId);

            cartAggregate.Cart.Name = request.ListName;

            return await SaveCartAsync(cartAggregate);
        }
    }
}
