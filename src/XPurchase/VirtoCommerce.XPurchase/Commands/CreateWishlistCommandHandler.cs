using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommandHandler : ScopedWishlistCommandHandlerBase<CreateWishlistCommand>
    {
        public CreateWishlistCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
        {
            request.CartType = XPurchaseConstants.ListTypeName;

            var cartAggregate = await CreateNewCartAggregateAsync(request);
            cartAggregate.Cart.Description = request.Description;
            await UpdateScopeAsync(cartAggregate, request);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
