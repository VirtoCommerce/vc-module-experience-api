using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartPaymentCommandHandler : CartCommandHandler<AddOrUpdateCartPaymentCommand>
    {
        private readonly ICartAvailMethodsService _cartAvailMethodService;

        public AddOrUpdateCartPaymentCommandHandler(ICartAggregateRepository cartRepository, ICartAvailMethodsService cartAvailMethodService)
            : base(cartRepository)
        {
            _cartAvailMethodService = cartAvailMethodService;
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartPaymentCommand request, CancellationToken cancellationToken)
        {
            ValidateAddressType(request.Payment.BillingAddress?.Value);
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var paymentId = request.Payment.Id?.Value ?? null;
            var payment = cartAggregate.Cart.Payments.FirstOrDefault(s => paymentId != null && s.Id == paymentId);
            payment = request.Payment.MapTo(payment);

            await cartAggregate.AddPaymentAsync(payment, await _cartAvailMethodService.GetAvailablePaymentMethodsAsync(cartAggregate));

            if (!request.Payment.DynamicProperties.IsNullOrEmpty())
            {
                await cartAggregate.UpdateCartPaymentDynamicProperties(payment, request.Payment.DynamicProperties);
            }

            return await SaveCartAsync(cartAggregate);
        }
    }
}
