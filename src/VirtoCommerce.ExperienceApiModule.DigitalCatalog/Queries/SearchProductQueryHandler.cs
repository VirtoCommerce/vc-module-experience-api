using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Facets;
using VirtoCommerce.XDigitalCatalog.Index;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Commands;
using ProductPrice = VirtoCommerce.ExperienceApiModule.Core.Models.ProductPrice;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductQueryHandler :
        IQueryHandler<SearchProductQuery, SearchProductResponse>,
        IQueryHandler<LoadProductQuery, LoadProductResponse>,
        IQueryHandler<LoadProductsQuery, LoadProductResponse>
    {
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IAggregationConverter _aggregationConverter;
        private readonly IInventorySearchService _inventorySearchService; // TODO: maybe we have __inventories in index
        private readonly ITaxProviderSearchService _taxProviderSearchService;
        private readonly IMarketingPromoEvaluator _marketingEvaluator;
        private readonly ICurrencyService _currencyService;
        private readonly IStoreService _storeService;
        private readonly IPricingService _pricingService;

        public SearchProductQueryHandler(
            ISearchProvider searchProvider
            , ISearchPhraseParser searchPhraseParser
            , IMapper mapper
            , IAggregationConverter aggregationConverter
            , ICartAggregateRepository cartAggregateRepository
            , IInventorySearchService inventorySearchService
            , IMarketingPromoEvaluator marketingEvaluator
            , ICurrencyService currencyService
            , IStoreService storeService
            , ITaxProviderSearchService taxProviderSearchService
            , IPricingService pricingService)
        {
            _searchProvider = searchProvider;
            _searchPhraseParser = searchPhraseParser;
            _mapper = mapper;
            _aggregationConverter = aggregationConverter;
            _cartAggregateRepository = cartAggregateRepository;
            _inventorySearchService = inventorySearchService;
            _marketingEvaluator = marketingEvaluator;
            _currencyService = currencyService;
            _taxProviderSearchService = taxProviderSearchService;
            _storeService = storeService;
            _pricingService = pricingService;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var indexIncludeFields = request.MapToIndexFields();

            var searchRequest = new SearchRequestBuilder(_searchPhraseParser, _aggregationConverter)
                .FromQuery(request)
                .ParseFilters(request.Filter)
                .ParseFacets(request.Facet, request.StoreId, request.CurrencyCode)
                .AddSorting(request.Sort)
                .WithIncludeFields(indexIncludeFields)
                .AddTerms(new[] { "status:visible" })//Only visible, exclude variations from search result
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            var expProducts = await ConvertIndexDocsToProductsWithLoadDependenciesAsync(searchResult.Documents, request);

            return new SearchProductResponse
            {
                Results = expProducts.ToList(),
                Facets = searchRequest.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, opts => opts.Items["aggregations"] = searchResult.Aggregations)).ToList(),
                TotalCount = (int)searchResult.TotalCount
            };
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductQuery request, CancellationToken cancellationToken)
        {
            var indexIncludeFields = request.MapToIndexFields();

            var searchRequest = new SearchRequestBuilder()
                .FromQuery(request)
                .WithIncludeFields(indexIncludeFields)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            var expProducts = await ConvertIndexDocsToProductsWithLoadDependenciesAsync(searchResult.Documents, request);

            return new LoadProductResponse(expProducts.ToList());
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductsQuery request, CancellationToken cancellationToken)
        {
            var indexIncludeFields = request.MapToIndexFields();

            var searchRequest = new SearchRequestBuilder()
                .FromQuery(request)
                .WithIncludeFields(indexIncludeFields)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            var expProducts = await ConvertIndexDocsToProductsWithLoadDependenciesAsync(searchResult.Documents, request);

            return new LoadProductResponse(expProducts.ToList());
        }

        protected virtual async Task<IEnumerable<ExpProduct>> ConvertIndexDocsToProductsWithLoadDependenciesAsync(IEnumerable<SearchDocument> docs, ICatalogQuery query)
        {
            //TODO: DRY with the same code in CartAggrRepository need to refactor in future
            var store = await _storeService.GetByIdAsync(query.StoreId);
            var defaultCultureName = store.DefaultLanguage ?? Language.InvariantLanguage.CultureName;
            //Clone currencies
            //TODO: Add caching  to prevent cloning each time
            var allCurrencies = (await _currencyService.GetAllCurrenciesAsync()).Select(x => x.Clone()).OfType<Currency>().ToArray();
            //Change culture name for all system currencies to requested
            allCurrencies.Apply(x => x.CultureName = query.CultureName ?? defaultCultureName);

            //Clone and change culture name for system currencies
            var currency = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(query.CurrencyCode ?? store.DefaultCurrency));
            if (currency == null)
            {
                throw new OperationCanceledException($"requested currency {query.CurrencyCode} is not registered in the system");
            }

            var result = docs?.Select(x => _mapper.Map<ExpProduct>(x, options =>
            {
                //TODO: Code duplication
                options.Items["all_currencies"] = allCurrencies;
                options.Items["store"] = store;
                options.Items["currency"] = currency;
            })).ToList() ?? new List<ExpProduct>();

            var productIds = result.Select(x => x.Id).ToArray();

            // If promotion evaluation requested
            if (query.HasPricingFields())
            {
                //TODO: !!!URGENT!!! Need to remove from here because we have introduced cycling dependency  with x-purchase project by these changes
                //Need to  replace to some loosely coupled solution based on Chain-Of-Responsibility pattern and special abstraction for build promotions evaluation  context independently
                var cartAggregate = await _cartAggregateRepository.GetCartAsync("default", query.StoreId, query.UserId, query.CultureName, query.CurrencyCode);
                if (cartAggregate == null)
                {
                    var createCartCommand = new CreateDefaultCartCommand(query.StoreId, query.UserId, query.CurrencyCode, query.CultureName);
                    var cart = _cartAggregateRepository.CreateDefaultShoppingCart(createCartCommand);
                    cartAggregate = await _cartAggregateRepository.GetCartForShoppingCartAsync(cart);
                }
                //evaluate prices only if product missed prices in the index storage
                var productsWithoutPrices = result.Where(x => !x.IndexedPrices.Any()).ToArray();
                if (productsWithoutPrices.Any())
                {
                    var pricesEvalContext = _mapper.Map<PriceEvaluationContext>(cartAggregate);
                    pricesEvalContext.ProductIds = productsWithoutPrices.Select(x => x.Id).ToArray();
                    var prices = await _pricingService.EvaluateProductPricesAsync(pricesEvalContext);
                    foreach (var product in productsWithoutPrices)
                    {
                        product.AllPrices = _mapper.Map<IEnumerable<ProductPrice>>(prices.Where(x => x.ProductId == product.Id), options =>
                        {
                            //TODO: Code duplication
                            options.Items["all_currencies"] = allCurrencies;
                            options.Items["store"] = store;
                            options.Items["currency"] = currency;
                        }).ToList();
                    }
                }

                //Evaluate promotions
                var promoEvalContext = _mapper.Map<PromotionEvaluationContext>(cartAggregate);
                promoEvalContext.PromoEntries = result.Select(x => _mapper.Map<ProductPromoEntry>(x, options =>
                {
                    //TODO: Code duplication
                    options.Items["all_currencies"] = allCurrencies;
                    options.Items["store"] = store;
                    options.Items["currency"] = currency;
                })).ToList();

                var promotionResults = await _marketingEvaluator.EvaluatePromotionAsync(promoEvalContext);
                var promoRewards = promotionResults.Rewards.OfType<CatalogItemAmountReward>().ToArray();
                if (promoRewards.Any())
                {
                    result.Apply(x => x.ApplyRewards(promoRewards));
                }
                //Evaluate taxes
                var storeTaxProviders = await _taxProviderSearchService.SearchTaxProvidersAsync(new TaxProviderSearchCriteria
                {
                    StoreIds = new[] { store.Id }
                });
                var activeTaxProvider = storeTaxProviders.Results.FirstOrDefault();
                if (activeTaxProvider != null)
                {
                    var taxEvalContext = _mapper.Map<TaxEvaluationContext>(cartAggregate);
                    taxEvalContext.Lines = result.SelectMany(x => _mapper.Map<IEnumerable<TaxLine>>(x)).ToList();
                    var taxRates = activeTaxProvider.CalculateRates(taxEvalContext);
                    if (taxRates.Any())
                    {
                        result.Apply(x => x.AllPrices.Apply(p => p.ApplyTaxRates(taxRates)));
                    }
                }
            }

            // If products availabilities requested
            if (query.HasInventoryFields())
            {
                var inventories = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                {
                    ProductIds = productIds,
                    //Do not use int.MaxValue use only 10 items per requested product
                    //TODO: Replace to pagination load
                    Take = Math.Min(productIds.Length * 10, 500)
                });
                if (inventories.Results.Any())
                {
                    result.Apply(x => x.ApplyStoreInventories(inventories.Results, store));
                }
            }
            return result;
        }

        //TODO: Need to remove at all!!
        public class CreateDefaultCartCommand : CartCommand
        {
            public CreateDefaultCartCommand(string storeId, string userId, string currency, string lang)//todo: change this to normal cartcommand
            : base(storeId, "cart", "default", userId, currency, lang)
            {
            }
        }
    }
}
