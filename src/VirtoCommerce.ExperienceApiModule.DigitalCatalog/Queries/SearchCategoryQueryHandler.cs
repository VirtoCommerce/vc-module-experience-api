using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQueryHandler : IQueryHandler<SearchCategoryQuery, SearchCategoryResponse>
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
            var searchRequest = new SearchRequestBuilder(_searchPhraseParser)
                .WithFuzzy(request.Fuzzy, request.FuzzyLevel)
                .ParseFilters(request.Filter)
                .ParseFacets(request.Facet)
                .WithSearchPhrase(request.Query)
                .WithPaging(request.Skip, request.Take)
                .AddSorting(request.Sort)
                //TODO: Remove hardcoded field name  __object from here
                .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Select(x => "__object." + x).ToArray())
                .AddObjectIds(request.CategoryIds)
                .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Category, searchRequest);

            return new SearchCategoryResponse
            {
                Results = searchResult.Documents?.Select(x => _mapper.Map<ExpCategory>(x)).ToList(),
                Facets = searchRequest.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, opts => opts.Items["aggregations"] = searchResult.Aggregations)).ToList(),
                TotalCount = (int)searchResult.TotalCount
            };
        }
    }
}
