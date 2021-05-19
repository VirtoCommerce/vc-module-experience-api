using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Commands
{
    public class UpdateCartPaymentDynamicPropertiesCommandHandler : CartCommandHandler<UpdateCartPaymentDynamicPropertiesCommand>
    {
        public UpdateCartPaymentDynamicPropertiesCommandHandler(ICartAggregateRepository cartRepository)
            : base(cartRepository)
        {
        }

        public override async Task<CartAggregate> Handle(UpdateCartPaymentDynamicPropertiesCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            await cartAggregate.UpdateCartPaymentDynamicProperties(request.PaymentId, request.DynamicProperties);

            return await SaveCartAsync(cartAggregate);
        }
    }
}
