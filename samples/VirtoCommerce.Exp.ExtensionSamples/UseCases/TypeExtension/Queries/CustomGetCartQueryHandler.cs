using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class CustomGetCartQueryHandler : GetCartQueryHandler
    {
        public CustomGetCartQueryHandler(ICartAggregateRepository cartAggrRepository)
            : base(cartAggrRepository)
        {
        }

        public override async Task<CartAggregate> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);
            result.Cart.ChannelId = "my-cool-channel";
            return result;
        }
    }
}
