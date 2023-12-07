using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XPurchase.Queries
{
    public class SearchWishlistQueryHandler : IQueryHandler<SearchWishlistQuery, SearchCartResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IMemberResolver _memberResolver;

        public SearchWishlistQueryHandler(
            ICartAggregateRepository cartAggregateRepository,
            IMapper mapper,
            ISearchPhraseParser searchPhraseParser,
            IMemberResolver memberResolver)
        {
            _cartAggregateRepository = cartAggregateRepository;
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
            _memberResolver = memberResolver;
        }

        public virtual async Task<SearchCartResponse> Handle(SearchWishlistQuery request, CancellationToken cancellationToken)
        {
            var contact = await _memberResolver.ResolveMemberByIdAsync(request.UserId) as Contact;
            var organizationId = contact?.Organizations?.FirstOrDefault();

            var searchCriteria = new CartSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                     .WithCurrency(request.CurrencyCode)
                                     .WithStore(request.StoreId)
                                     .WithType(XPurchaseConstants.ListTypeName)
                                     .WithLanguage(request.CultureName)
                                     .WithCustomerId(request.UserId)
                                     .WithOrganizationId(organizationId)
                                     .WithScope(request.Scope)
                                     .WithPaging(request.Skip, request.Take)
                                     .WithSorting(request.Sort)
                                     .WithResponseGroup(CartResponseGroup.WithLineItems)
                                     .Build();

            return await _cartAggregateRepository.SearchCartAsync(searchCriteria);
        }
    }
}
