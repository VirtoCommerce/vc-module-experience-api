using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class CreateCartCommandHandler : CartCommandHandler<CreateCartCommand>
    {
        public CreateCartCommandHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CreateNewCartAggregateAsync(request);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
