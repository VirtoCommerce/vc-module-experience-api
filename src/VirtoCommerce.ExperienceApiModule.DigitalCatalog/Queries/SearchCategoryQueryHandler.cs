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
    public class SearchCategoryQueryHandler :
        IQueryHandler<SearchCategoryQuery, SearchCategoryResponse>
        , IQueryHandler<LoadCategoryQuery, LoadCategoryResponce>
    {
        private readonly IMapper _mapper;
        private readonly ISearchProvider _searchProvider;
        private readonly IRequestBuilder _requestBuilder;

        public SearchCategoryQueryHandler(
            ISearchProvider searchProvider
            , IMapper mapper
            , IRequestBuilder requestBuilder)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
            _requestBuilder = requestBuilder;
        }

        public virtual async Task<SearchCategoryResponse> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
        {
            var searchRequest = _requestBuilder
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
            var searchRequest = _requestBuilder
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
