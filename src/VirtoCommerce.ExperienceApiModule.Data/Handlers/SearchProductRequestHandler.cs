using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.ExperienceApiModule.Data.Index;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
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
                                            .WithPaging(request.Skip, request.Take)
                                            .AddSorting(request.Sort)
                                            .AddSearchKeyword(request.Keyword)
                                            .AddTerms(request.Terms)
                                            .WithIncludeFields(request.IncludeFields)
                                            .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            result.Result.Results = searchResult.Documents.Select(x => x.Materialize<CatalogProduct>()).ToList(); 
            result.Result.TotalCount = (int)searchResult.TotalCount;
            return result;
        }
    }
}
