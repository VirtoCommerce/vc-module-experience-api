using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class ValidateCouponQueryHandler : IQueryHandler<ValidateCouponQuery, bool>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;

        public ValidateCouponQueryHandler(ICartAggregateRepository cartAggregateRepository)
        {
            _cartAggregateRepository = cartAggregateRepository;
        }

        public async Task<bool> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
        {
            var cartAggregate = await GetCartAggregateAsync(request);

            if (cartAggregate != null)
            {
                var clonedCartAggrerate = cartAggregate.Clone() as CartAggregate;
                clonedCartAggrerate.Cart.Coupons = new[] { request.Coupon };

                return await clonedCartAggrerate.ValidateCouponAsync(request.Coupon);
            }

            return false;
        }

        protected virtual Task<CartAggregate> GetCartAggregateAsync(ValidateCouponQuery request)
        {
            return _cartAggregateRepository.GetCartAsync(request.CartName, request.StoreId, request.UserId, request.CultureName, request.CurrencyCode, request.CartType);
        }
    }
}
