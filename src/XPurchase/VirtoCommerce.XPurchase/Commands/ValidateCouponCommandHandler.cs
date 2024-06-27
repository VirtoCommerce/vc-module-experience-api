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

        public virtual async Task<bool> Handle(ValidateCouponCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetCartAggregateFromCommandAsync(request);

            if (cartAggregate != null)
            {
                var clonedCartAggrerate = cartAggregate.Clone() as CartAggregate;
                clonedCartAggrerate.Cart.Coupons = new[] { request.Coupon };

                return await clonedCartAggrerate.ValidateCouponAsync(request.Coupon);
            }

            return false;
        }

        protected virtual Task<CartAggregate> GetCartAggregateFromCommandAsync(ValidateCouponCommand request)
        {
            return CartAggrRepository.GetCartAsync(request.CartName, request.StoreId, request.UserId, request.OrganizationId, request.CultureName, request.CurrencyCode, request.CartType);
        }
    }
}
