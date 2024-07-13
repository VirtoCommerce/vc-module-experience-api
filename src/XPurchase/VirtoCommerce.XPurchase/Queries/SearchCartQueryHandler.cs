using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchCartQueryHandler : IQueryHandler<SearchCartQuery, SearchCartResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly ICartResponseGroupParser _cartResponseGroupParser;

        public SearchCartQueryHandler(
            ICartAggregateRepository cartAggregateRepository,
            IMapper mapper,
            ISearchPhraseParser searchPhraseParser,
            ICartResponseGroupParser cartResponseGroupParser)
        {
            _cartAggregateRepository = cartAggregateRepository;
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
            _cartResponseGroupParser = cartResponseGroupParser;
        }

        public virtual async Task<SearchCartResponse> Handle(SearchCartQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.CartId))
            {
                var result = new SearchCartResponse();
                if (request.Skip > 0)
                {
                    return result;
                }

                var cart = await _cartAggregateRepository.GetCartByIdAsync(request.CartId, GetResponseGroup(request), request.IncludeFields.ItemsToProductIncludeField(), request.CultureName);
                if (cart is not null)
                {
                    result.TotalCount = 1;
                    if (request.Take > 0)
                    {
                        result.Results.Add(cart);
                    }
                }

                return result;
            }

            var searchCriteria = GetSearchCriteria(request);

            return await _cartAggregateRepository.SearchCartAsync(searchCriteria);
        }

        protected virtual ShoppingCartSearchCriteria GetSearchCriteria(SearchCartQuery request)
        {
            var criteria = new CartSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                     .ParseFilters(request.Filter)
                                     .WithCurrency(request.CurrencyCode)
                                     .WithStore(request.StoreId)
                                     .WithType(request.CartType)
                                     .WithLanguage(request.CultureName)
                                     .WithCustomerId(request.UserId)
                                     .WithOrganizationId(request.OrganizationId)
                                     .WithPaging(request.Skip, request.Take)
                                     .WithSorting(request.Sort)
                                     .Build();
            criteria.ResponseGroup = GetResponseGroup(request);

            return criteria;
        }

        private string GetResponseGroup(SearchCartQuery request)
        {
            return EnumUtility.SafeParseFlags(_cartResponseGroupParser.GetResponseGroup(request.IncludeFields), CartResponseGroup.Full).ToString();
        }
    }
}
