using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
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
        private readonly IStoreCurrencyResolver _storeCurrencyResolver;
        private readonly IStoreService _storeService;
        private readonly IGenericPipelineLauncher _pipeline;
        private readonly IAggregationConverter _aggregationConverter;
        private readonly ISearchPhraseParser _phraseParser;

        public SearchProductQueryHandler(
            ISearchProvider searchProvider
            , IMapper mapper
            , IStoreCurrencyResolver storeCurrencyResolver
            , IStoreService storeService
            , IGenericPipelineLauncher pipeline
            , IAggregationConverter aggregationConverter
            , ISearchPhraseParser phraseParser)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
            _storeCurrencyResolver = storeCurrencyResolver;
            _storeService = storeService;
            _pipeline = pipeline;
            _aggregationConverter = aggregationConverter;
            _phraseParser = phraseParser;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var allStoreCurrencies = await _storeCurrencyResolver.GetAllStoreCurrenciesAsync(request.StoreId, request.CultureName);
            var currency = await _storeCurrencyResolver.GetStoreCurrencyAsync(request.CurrencyCode, request.StoreId, request.CultureName);
            var store = await _storeService.GetByIdAsync(request.StoreId);
            var responseGroup = EnumUtility.SafeParse(request.GetResponseGroup(), ExpProductResponseGroup.None);

            var builder = new IndexSearchRequestBuilder()
                                            .WithFuzzy(request.Fuzzy, request.FuzzyLevel)
                                            .ParseFilters(_phraseParser, request.Filter)
                                            .WithSearchPhrase(request.Query)
                                            .WithPaging(request.Skip, request.Take)
                                            .AddObjectIds(request.ObjectIds)
                                            .AddSorting(request.Sort)
                                            .WithIncludeFields(IndexFieldsMapper.MapToIndexIncludes(request.IncludeFields).ToArray());

            if (request.ObjectIds.IsNullOrEmpty())
            {
                //filter products only the store catalog and visibility status when search
                builder.AddTerms(new[] { "status:visible" });//Only visible, exclude variations from search result
                builder.AddTerms(new[] { $"__outline_named:{store.Catalog}" });
            }

            //Use predefined  facets for store  if the facet filter expression is not set
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadFacets))
            {
                var predefinedAggregations = await _aggregationConverter.GetAggregationRequestsAsync(new ProductIndexedSearchCriteria
                {
                    StoreId = request.StoreId,
                    Currency = request.CurrencyCode,
                }, new FiltersContainer());

                builder.ParseFacets(_phraseParser, request.Facet, predefinedAggregations)
                       .ApplyMultiSelectFacetSearch();
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

            // For every request aggregation, check the filter values that exist in the result.
            // If the value really exist, then fire on IsApplied
            foreach (var requestAggregation in searchRequest.Aggregations)
            {
                var resultAggregation = aggregations.FirstOrDefault(x => x.Field == requestAggregation.FieldName);
                if (resultAggregation != null)
                {
                    foreach (var resultAggregationValue in resultAggregation.Items.Where(x => ((AndFilter)requestAggregation.Filter).ChildFilters.Any(y => ((TermFilter)y).Values.Contains(x.Value))))
                    {
                        resultAggregationValue.IsApplied = true;
                    }
                }
            }

            var products = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x)).ToList() ?? new List<ExpProduct>();

            var result = new SearchProductResponse
            {
                Query = request,
                AllStoreCurrencies = allStoreCurrencies,
                Currency = currency,
                Store = store,
                Results = products,
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
