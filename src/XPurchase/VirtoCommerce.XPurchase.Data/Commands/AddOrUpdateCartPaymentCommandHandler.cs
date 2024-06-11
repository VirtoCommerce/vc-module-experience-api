using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
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
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var paymentId = request.Payment.Id?.Value ?? null;
            var payment = cartAggregate.Cart.Payments.FirstOrDefault(s => paymentId != null && s.Id == paymentId);
            payment = request.Payment.MapTo(payment);

            await cartAggregate.AddPaymentAsync(payment, await _cartAvailMethodService.GetAvailablePaymentMethodsAsync(cartAggregate));

            if (!request.Payment.DynamicProperties.IsNullOrEmpty())
            {
                await cartAggregate.UpdateCartPaymentDynamicProperties(payment, request.Payment.DynamicProperties);
            }

            cartAggregate = await SaveCartAsync(cartAggregate);
            return await GetCartById(cartAggregate.Cart.Id, request.CultureName);
        }
    }
}
