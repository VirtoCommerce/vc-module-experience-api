using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using VirtoCommerce.XDigitalCatalog.Extensions;
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
                AddDefaultTerms(builder, store.Catalog);
            }

            var criteria = new ProductIndexedSearchCriteria
            {
                StoreId = request.StoreId,
                Currency = request.CurrencyCode,
                LanguageCode = store.Languages.Contains(request.CultureName) ? request.CultureName : store.DefaultLanguage,
                CatalogId = store.Catalog
            };

            //Use predefined  facets for store  if the facet filter expression is not set
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadFacets))
            {
                var predefinedAggregations = await _aggregationConverter.GetAggregationRequestsAsync(criteria, new FiltersContainer());

                // Note: Add to the facet phrase language-specific facet name in a hope the sought facet can be made by non-dictionary, multivalue and multilanguage property.
                // See details: PT-3517
                var facets = string.Empty;
                foreach (var facet in request.Facet?.Split(" ") ?? new string[0])
                {
                    facets = $"{facets} {facet} {facet}_{criteria.LanguageCode.ToLowerInvariant()}";
                }
                builder.ParseFacets(_phraseParser, facets, predefinedAggregations)
                   .ApplyMultiSelectFacetSearch();

            }

            var searchRequest = builder.Build();
            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            //TODO: move later to own implementation
            //Call the catalog aggregation converter service to convert AggregationResponse to proper Aggregation type (term, range, filter)
            var resultAggregations = await _aggregationConverter.ConvertAggregationsAsync(searchResult.Aggregations, criteria);

            searchRequest.SetAppliedAggregations(resultAggregations.ToArray());

            var products = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x)).ToList() ?? new List<ExpProduct>();

            var result = new SearchProductResponse
            {
                Query = request,
                AllStoreCurrencies = allStoreCurrencies,
                Currency = currency,
                Store = store,
                Results = products,
                Facets = resultAggregations?.Select(x =>
                    {
                        // Apply language-specific facet result
                        // To do this, copy facet items from the fake language-specific facet to the real facet
                        var languageSpecificAggregation = resultAggregations.FirstOrDefault(y => y.Field == $"{x.Field}_{criteria.LanguageCode.ToLowerInvariant()}");
                        if (languageSpecificAggregation != null)
                            x.Items = languageSpecificAggregation.Items;
                        return x;
                    })
                    .Where(x => !Regex.IsMatch(x.Field, @"_\w\w-\w\w$", RegexOptions.IgnoreCase)) // Drop fake language-specific facets from results
                    .Select(x => _mapper.Map<FacetResult>(x, options =>
                    {
                        options.Items["cultureName"] = criteria.LanguageCode;
                    })).ToList(),
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
        /// By default limit  resulting products, return only visible products and belongs to store catalog,
        /// but user can override this behaviour by passing "status:hidden" in a filter expression
        /// </summary>
        /// <param name="builder">Instance of the request builder</param>
        /// <param name="catalog">Name of the current catalog</param>
        protected virtual void AddDefaultTerms(IndexSearchRequestBuilder builder, string catalog)
        {
            builder.AddTerms(new[] { "status:visible" }, skipIfExists: true);
            builder.AddTerms(new[] { $"__outline:{catalog}" });
        }
    }
}
