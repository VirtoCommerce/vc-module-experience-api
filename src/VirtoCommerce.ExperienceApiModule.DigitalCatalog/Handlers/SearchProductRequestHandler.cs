using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Binders;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Handlers
{
    public class SearchProductRequestHandler : IRequestHandler<SearchProductRequest, SearchProductResponse>
    {
        private readonly ISearchProvider _searchProvider;
        private readonly ISearchPhraseParser _searchPhraseParser;
        public SearchProductRequestHandler(ISearchProvider searchProvider, ISearchPhraseParser searchPhraseParser)
        {
            _searchProvider = searchProvider;
            _searchPhraseParser = searchPhraseParser;
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductRequest request, CancellationToken cancellationToken)
        {
            var result = new SearchProductResponse();
            var searchRequest = new SearchRequestBuilder(_searchPhraseParser)
                                            .WithFuzzy(request.Fuzzy)
                                            .ParseFilters(request.Filter)
                                            .WithSearchPhrase(request.Query)
                                            .WithPaging(request.Skip, request.Take)
                                            .AddSorting(request.Sort)
                                            //TODO: Remove hardcoded field name  __object from here
                                            .WithIncludeFields(request.IncludeFields.Concat(new[] { "id" }).Select(x => "__object." + x).ToArray())
                                            .WithIncludeFields(request.IncludeFields.Where(x=>x.StartsWith("prices.")).Concat(new[] { "id" }).Select(x => "__prices." + x.TrimStart("prices.")).ToArray())
                                            .Build();
                                           
            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            var binder = new ProductBinder();
            result.Results = searchResult.Documents.Select(x => binder.BindModel(x)).OfType<ExpProduct>().ToList(); 
            result.TotalCount = (int)searchResult.TotalCount;
            return result;
        }
    }
}
