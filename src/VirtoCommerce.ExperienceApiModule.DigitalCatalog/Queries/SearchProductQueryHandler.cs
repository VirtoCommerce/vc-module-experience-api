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
            var resultAggregations = await _aggregationConverter.ConvertAggregationsAsync(searchResult.Aggregations, criteria);

            SetAppliedAggregations(searchRequest, resultAggregations);

            var products = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x)).ToList() ?? new List<ExpProduct>();

            var result = new SearchProductResponse
            {
                Query = request,
                AllStoreCurrencies = allStoreCurrencies,
                Currency = currency,
                Store = store,
                Results = products,
                Facets = resultAggregations?.Select(x => _mapper.Map<FacetResult>(x)).ToList(),
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

        /// <summary>
        /// For every request aggregation, check the filter values that exist in the result.
        /// If the value really exists, then set on IsApplied
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <param name="resultAggregations"></param>
        private static void SetAppliedAggregations(SearchRequest searchRequest, Aggregation[] resultAggregations)
        {
            foreach (var childFilter in (searchRequest.Filter as AndFilter)?.ChildFilters ?? Enumerable.Empty<IFilter>())
            {
                foreach (var resultAggregation in resultAggregations.Where(x => x.Field == ((INamedFilter)childFilter).FieldName.Split('_')[0] /* TermFilter names are equal, RangeFilter can contain underscore in the name */))
                {
                    foreach (var resultAggregationValue in resultAggregation.Items)
                    {
                        switch (childFilter)
                        {
                            case TermFilter termFilter:
                                // For term filters: just check result value in filter values
                                resultAggregationValue.IsApplied = termFilter.Values.Contains(resultAggregationValue.Value);
                                break;
                            case RangeFilter rangeFilter:
                                // For range filters check the values have the same bounds
                                resultAggregationValue.IsApplied = rangeFilter.Values.Any(z =>
                                    (z.Lower ?? string.Empty) == (resultAggregationValue.RequestedLowerBound ?? string.Empty) &&
                                    (z.Upper ?? string.Empty) == (resultAggregationValue.RequestedUpperBound ?? string.Empty));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
