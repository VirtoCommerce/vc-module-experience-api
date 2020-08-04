using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.XPurchase.Queries
{
    public class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartAggregate>, IQueryHandler<GetCartByIdQuery, CartAggregate>, IQueryHandler<GetWishListQuery, IList<WishList>>
    {
        private readonly ICartAggregateRepository _cartAggrRepository;

        public GetCartQueryHandler(ICartAggregateRepository cartAggrRepository)
        {
            _cartAggrRepository = cartAggrRepository;
        }

        public Task<CartAggregate> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
           return  _cartAggrRepository.GetCartAsync(request.CartName, request.StoreId, request.UserId, request.CultureName, request.CurrencyCode, request.CartType);

        }

        public Task<CartAggregate> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            return _cartAggrRepository.GetCartByIdAsync(request.CartId);

        }

        public Task<IList<WishList>> Handle(GetWishListQuery request, CancellationToken cancellationToken)
        {
            return _cartAggrRepository.GetWishesListAsync(request.StoreId, request.UserId, request.CultureName, request.CurrencyCode, request.CartType);
        }
    }
}
