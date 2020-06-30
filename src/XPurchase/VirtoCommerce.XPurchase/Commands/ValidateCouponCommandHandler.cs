using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.XPurchase.Commands
{
    public class ValidateCouponCommandHandler : IRequestHandler<ValidateCouponCommand, bool>
    {
        public ValidateCouponCommandHandler(ICartAggregateRepository cartAggrRepository)
        {
            CartAggrRepository = cartAggrRepository;
        }

        private ICartAggregateRepository CartAggrRepository { get; set; }

        public async Task<bool> Handle(ValidateCouponCommand request, CancellationToken cancellationToken)
        {
            var cartAggr = await GetCartAggregateFromCommandAsync(request);
            var isValid = await cartAggr.ValidateCouponAsync(request.Coupon);
            return isValid;
        }

        protected Task<CartAggregate> GetCartAggregateFromCommandAsync(ValidateCouponCommand request)
        {
            return CartAggrRepository.GetOrCreateAsync(request.CartName, request.StoreId, request.UserId, request.Language, request.Currency, request.CartType);
        }
    }
}
