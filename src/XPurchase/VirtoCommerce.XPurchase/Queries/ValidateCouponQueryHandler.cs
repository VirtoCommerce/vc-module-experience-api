using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public class ValidateCouponQueryHandler : IQueryHandler<ValidateCouponQuery, bool>
    {
        private readonly IMediator _mediator;

        public ValidateCouponQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
        {
            var getCartQuery = new GetCartQuery(request);
            var cartAggregate = await _mediator.Send(getCartQuery, cancellationToken);

            if (cartAggregate != null)
            {
                var clonedCartAggrerate = cartAggregate.Clone() as CartAggregate;
                clonedCartAggrerate.Cart.Coupons = new[] { request.Coupon };

                return await clonedCartAggrerate.ValidateCouponAsync(request.Coupon);
            }

            return false;
        }
    }
}
