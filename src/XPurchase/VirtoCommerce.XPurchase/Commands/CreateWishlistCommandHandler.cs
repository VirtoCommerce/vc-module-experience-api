using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateWishlistCommandHandler : CartCommandHandler<CreateWishlistCommand>
    {
        public CreateWishlistCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
        {
            request.CartType = XPurchaseConstants.ListTypeName;

            var cartAggregate = await CreateNewCartAggregateAsync(request);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
