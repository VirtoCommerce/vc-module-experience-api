using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateCartCommandHandler : CartCommandHandler<CreateCartCommand>
    {
        public CreateCartCommandHandler(ICartAggregateRepository cartAggregateRepository)
            : base(cartAggregateRepository)
        {
        }

        public override async Task<CartAggregate> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CreateNewCartAggregateAsync(request);

            cartAggregate.Cart.OrganizationId = request.OrganizationId;

            return await SaveCartAsync(cartAggregate);
        }
    }
}
