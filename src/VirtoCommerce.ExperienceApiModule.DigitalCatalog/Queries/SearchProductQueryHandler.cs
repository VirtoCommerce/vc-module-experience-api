using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductQueryHandler : IQueryHandler<SearchProductQuery, SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IAggregationConverter _aggregationConverter;

        public SearchProductQueryHandler(ISearchProvider searchProvider, ISearchPhraseParser searchPhraseParser, IMapper mapper, IAggregationConverter aggregationConverter)
        {
            _searchProvider = searchProvider;
            _searchPhraseParser = searchPhraseParser;
            _mapper = mapper;
            _aggregationConverter = aggregationConverter;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var searchRequest = new SearchRequestBuilder(_searchPhraseParser, _aggregationConverter)
                .WithFuzzy(request.Fuzzy, request.FuzzyLevel)
                .ParseFilters(request.Filter)
                .ParseFacets(request.Facet, request.StoreId)
                .WithSearchPhrase(request.Query)
                .WithPaging(request.Skip, request.Take)
                .AddSorting(request.Sort)
                //TODO: Remove hardcoded field name  __object from here
                .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Select(x => "__object." + x).ToArray())
                .WithIncludeFields(request.IncludeFields.Where(x => x.StartsWith("prices.")).Concat(new[] { "id" }).Select(x => "__prices." + x.TrimStart("prices.")).ToArray())
                .WithIncludeFields(request.IncludeFields.Any(x => x.StartsWith("variations."))
                    ? new[] { "__variations" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.StartsWith("category."))
                    ? new[] { "__object.categoryId" }
                    : Array.Empty<string>())
                // Add master variation fields
                .WithIncludeFields(request.IncludeFields
                    .Where(x => x.StartsWith("masterVariation."))
                    .Concat(new[] { "mainProductId" })
                    .Select(x => "__object." + x.TrimStart("masterVariation."))
                    .ToArray())
                // Add seoInfos
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("slug", StringComparison.OrdinalIgnoreCase)
                                                                || x.Contains("meta", StringComparison.OrdinalIgnoreCase)) // for metaKeywords, metaTitle and metaDescription
                    ? new[] { "__object.seoInfos" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("imgSrc", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.images" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("brandName", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.properties" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("descriptions", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.reviews" }
                    : Array.Empty<string>())
                .WithIncludeFields(request.IncludeFields.Any(x => x.Contains("availabilityData", StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.isActive", "__object.isBuyable", "__object.trackInventory" }
                    : Array.Empty<string>())
                .AddObjectIds(request.ProductIds)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);

            return new SearchProductResponse
            {
                Results = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x)).ToList(),
                Facets = searchRequest.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, opts => opts.Items["aggregations"] = searchResult.Aggregations)).ToList(),
                TotalCount = (int)searchResult.TotalCount
            };
        }
    }
}
