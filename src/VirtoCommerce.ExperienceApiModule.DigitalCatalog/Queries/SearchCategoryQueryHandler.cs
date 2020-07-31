using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Facets;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQueryHandler
        : IQueryHandler<SearchCategoryQuery, SearchCategoryResponse>
        , IQueryHandler<LoadCategoryQuery, LoadCategoryResponce>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _searchPhraseParser;

        public SearchCategoryQueryHandler(ISearchProvider searchProvider, ISearchPhraseParser searchPhraseParser, IMapper mapper)
        {
            _searchProvider = searchProvider;
            _searchPhraseParser = searchPhraseParser;
            _mapper = mapper;
        }

        public virtual async Task<SearchCategoryResponse> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
        {
            var searchRequest = new SearchRequestBuilder(_searchPhraseParser, null)
                .FromQuery(request)
                .ParseFilters(request.Filter)
                .ParseFacets(request.Facet)
                .AddSorting(request.Sort)
                //TODO: Remove hardcoded field name  __object from here
                .WithIncludeFields(request.IncludeFields.Concat(new[] { "id", "parentId" }).Select(x => "__object." + x).ToArray())
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Category, searchRequest);

            return new SearchCategoryResponse
            {
                Results = searchResult.Documents?.Select(x => _mapper.Map<ExpCategory>(x)).ToList(),
                Facets = searchRequest.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, opts => opts.Items["aggregations"] = searchResult.Aggregations)).ToList(),
                TotalCount = (int)searchResult.TotalCount
            };
        }

        public virtual async Task<LoadCategoryResponce> Handle(LoadCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = new LoadCategoryResponce();
            var searchRequest = new SearchRequestBuilder()
                .FromQuery(request)
                .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Distinct().Select(x => $"__object.{x}").ToArray())
                .WithIncludeFields((request.IncludeFields.Any(x => x.Contains("slug", System.StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.seoInfos" }
                    : Enumerable.Empty<string>()).ToArray())
                .WithIncludeFields((request.IncludeFields.Any(x => x.Contains("parent", System.StringComparison.OrdinalIgnoreCase))
                    ? new[] { "__object.parentId" }
                    : Enumerable.Empty<string>()).ToArray())
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Category, searchRequest);
            result.Category = searchResult.Documents.Select(x => _mapper.Map<ExpCategory>(x)).FirstOrDefault();

            return result;
        }
    }
}
