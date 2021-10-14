using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Commands
{
    public class AddOrUpdateCartPaymentCommandHandler : CartCommandHandler<AddOrUpdateCartPaymentCommand>
    {
        private readonly ICartAvailMethodsService _cartAvailMethodService;
        private readonly IMapper _mapper;

        public AddOrUpdateCartPaymentCommandHandler(ICartAggregateRepository cartRepository, ICartAvailMethodsService cartAvailMethodService, IMapper mapper)
            : base(cartRepository)
        {
            _cartAvailMethodService = cartAvailMethodService;
            _mapper = mapper;
        }

        public override async Task<CartAggregate> Handle(AddOrUpdateCartPaymentCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetOrCreateCartFromCommandAsync(request);

            var paymentId = request.Payment.Id?.Value ?? null;
            var payment = cartAggregate.Cart.Payments.FirstOrDefault(s => paymentId != null && s.Id == paymentId);
            payment = _mapper.Map(request.Payment, payment);

            await cartAggregate.AddPaymentAsync(payment, await _cartAvailMethodService.GetAvailablePaymentMethodsAsync(cartAggregate));

            return await SaveCartAsync(cartAggregate);
        }
    }
}
