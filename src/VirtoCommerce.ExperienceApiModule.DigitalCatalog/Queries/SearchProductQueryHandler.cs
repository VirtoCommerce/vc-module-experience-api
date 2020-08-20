using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductQueryHandler : IQueryHandler<SearchProductQuery, SearchProductResponse>, IQueryHandler<LoadProductsQuery, LoadProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly IRequestBuilder _requestBuilder;
        private readonly IStoreCurrencyResolver _storeCurrencyResolver;
        private readonly IStoreService _storeService;
        private readonly IGenericPipelineLauncher _pipeline;
        private readonly IAggregationConverter _aggregationConverter;
        public SearchProductQueryHandler(
            ISearchProvider searchProvider
            , IRequestBuilder requestBuilder
            , IMapper mapper
            , IStoreCurrencyResolver storeCurrencyResolver
            , IStoreService storeService
            , IGenericPipelineLauncher pipeline
            , IAggregationConverter aggregationConverter)
        {
            _searchProvider = searchProvider;
            _requestBuilder = requestBuilder;
            _mapper = mapper;
            _storeCurrencyResolver = storeCurrencyResolver;
            _storeService = storeService;
            _pipeline = pipeline;
            _aggregationConverter = aggregationConverter;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var builder = _requestBuilder
                .FromQuery(request)
                .ParseFacets(request.Facet, request.StoreId, request.CurrencyCode);
         
            if (request.ObjectIds.IsNullOrEmpty())
            {
                builder.AddTerms(new[] { "status:visible" });//Only visible, exclude variations from search result
            }
            var searchRequest = builder.Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            var criteria = new ProductIndexedSearchCriteria
            {
                StoreId = request.StoreId,
                Currency = request.CurrencyCode,
            };
            //TODO: move later to own implementation
            //Call the catalog aggregation converter service to convert AggregationResponse to proper Aggregation type (term, range, filter)
            var aggregations = await _aggregationConverter.ConvertAggregationsAsync(searchResult.Aggregations, criteria);


            var allStoreCurrencies = await _storeCurrencyResolver.GetAllStoreCurrenciesAsync(request.StoreId, request.CultureName);
            var currency = await _storeCurrencyResolver.GetStoreCurrencyAsync(request.CurrencyCode, request.StoreId, request.CultureName);
            var store = await _storeService.GetByIdAsync(request.StoreId);

            var products = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x, options =>
            {
                options.Items["all_currencies"] = allStoreCurrencies;
                options.Items["store"] = store;
                options.Items["currency"] = currency;
            })).ToList() ?? new List<ExpProduct>();

            var result = new SearchProductResponse
            {
                Query = request,
                AllStoreCurrencies = allStoreCurrencies,
                Currency = currency,
                Store = store,
                Results = products.ToList(),
                Facets = aggregations?.Select(x => _mapper.Map<FacetResult>(x)).ToList(),
                TotalCount = (int)searchResult.TotalCount
            };

            await _pipeline.Execute(result);

            return result;
        }
     
        public virtual async Task<LoadProductResponse> Handle(LoadProductsQuery request, CancellationToken cancellationToken)
        {
            var searchRequest = _mapper.Map<SearchProductQuery>(request);

            var result = await Handle(searchRequest, cancellationToken);

            return new LoadProductResponse(result.Results);
        }


    }
}
