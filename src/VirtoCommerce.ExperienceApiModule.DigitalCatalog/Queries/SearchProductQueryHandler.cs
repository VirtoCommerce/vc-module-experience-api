using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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

        public SearchProductQueryHandler(ISearchProvider searchProvider, ISearchPhraseParser searchPhraseParser, IMapper mapper)
        {
            _searchProvider = searchProvider;
            _searchPhraseParser = searchPhraseParser;
            _mapper = mapper;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var result = new SearchProductResponse();
            var searchRequest = new SearchRequestBuilder(_searchPhraseParser)
                                            .WithFuzzy(request.Fuzzy, request.FuzzyLevel)
                                            .ParseFilters(request.Filter)
                                            .ParseFacets(request.Facet)
                                            .WithSearchPhrase(request.Query)
                                            .WithPaging(request.Skip, request.Take)
                                            .AddSorting(request.Sort)
                                            //TODO: Remove hardcoded field name  __object from here
                                            .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Select(x => "__object." + x).ToArray())
                                            .WithIncludeFields(request.IncludeFields.Where(x => x.StartsWith("prices.")).Concat(new[] { "id" }).Select(x => "__prices." + x.TrimStart("prices.")).ToArray())
                                            .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            result.Results = searchResult.Documents?.Select(x => _mapper.Map<ExpProduct>(x)).ToList();
            result.Facets = searchRequest.Aggregations?.Select(x => _mapper.Map<FacetResult>(x, opts => opts.Items["aggregations"] = searchResult.Aggregations)).ToList();

            result.TotalCount = (int)searchResult.TotalCount;
            return result;
        }
    }
}
