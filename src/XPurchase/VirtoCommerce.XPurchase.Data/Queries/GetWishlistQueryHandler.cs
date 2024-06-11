using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Queries;
using VirtoCommerce.XPurchase.Core.Services;
using VirtoCommerce.XPurchase.Data.Extensions;

namespace VirtoCommerce.XPurchase.Data.Queries
{
    public class GetWishlistQueryHandler : IQueryHandler<GetWishlistQuery, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartAggrRepository;

        public GetWishlistQueryHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartAggrRepository = cartAggrRepository;
        }

        public Task<CartAggregate> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            return _cartAggrRepository.GetCartByIdAsync(request.ListId, request.IncludeFields.ItemsToProductIncludeField(), language: request.CultureName);
        }
    }
}
