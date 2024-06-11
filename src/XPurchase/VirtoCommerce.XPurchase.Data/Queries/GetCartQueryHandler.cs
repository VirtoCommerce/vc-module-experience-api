using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Queries;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Queries
{
    public class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartAggregate>, IQueryHandler<GetCartByIdQuery, CartAggregate>
    {
        private readonly ICartAggregateRepository _cartAggrRepository;
        private readonly ICartResponseGroupParser _cartResponseGroupParser;

        public GetCartQueryHandler(ICartAggregateRepository cartAggrRepository, ICartResponseGroupParser cartResponseGroupParser)
        {
            _cartAggrRepository = cartAggrRepository;
            _cartResponseGroupParser = cartResponseGroupParser;
        }

        public virtual Task<CartAggregate> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cartSearchCriteria = GetCartSearchCriteria(request);

            return _cartAggrRepository.GetCartAsync(cartSearchCriteria, request.CultureName);
        }

        public virtual Task<CartAggregate> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            return _cartAggrRepository.GetCartByIdAsync(request.CartId);
        }


        protected virtual ShoppingCartSearchCriteria GetCartSearchCriteria(GetCartQuery request)
        {
            var cartSearchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();

            cartSearchCriteria.StoreId = request.StoreId;
            cartSearchCriteria.CustomerId = request.UserId;
            cartSearchCriteria.Name = request.CartName;
            cartSearchCriteria.Currency = request.CurrencyCode;
            cartSearchCriteria.Type = request.CartType;
            cartSearchCriteria.ResponseGroup = EnumUtility.SafeParseFlags(_cartResponseGroupParser.GetResponseGroup(request.IncludeFields), CartResponseGroup.Full).ToString();

            return cartSearchCriteria;
        }
    }
}
