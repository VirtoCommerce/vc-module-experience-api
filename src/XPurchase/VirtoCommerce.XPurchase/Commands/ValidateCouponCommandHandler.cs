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
            var cartAggregate = await GetCartAggregateFromCommandAsync(request);

            if (cartAggregate != null)
            {
                return await cartAggregate.ValidateCouponAsync(request.Coupon);
            }

            return false;
        }

        protected Task<CartAggregate> GetCartAggregateFromCommandAsync(ValidateCouponCommand request)
        {
            return CartAggrRepository.GetCartAsync(request.CartName, request.StoreId, request.UserId, request.Language, request.Currency, request.CartType);
        }
    }
}
