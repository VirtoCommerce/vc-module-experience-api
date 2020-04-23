using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.ExperienceApiModule.Data.Index;
using VirtoCommerce.Platform.Core.Common;
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
            var requestBuilder = new SearchRequestBuilder(_searchPhraseParser)
                                            .ParseFilters(request.Query)
                                            .WithPaging(request.Skip, request.Take)
                                            .AddObjectIds(request.ObjectIds)
                                            .AddSorting(request.Sort)
                                            //TODO: Remove hardcoded field name  __object from here
                                            .WithIncludeFields(request.IncludeFields.Select(x => "__object." + x).ToArray())
                                            //TODO: How to include fields that have different names that object???
                                            .WithIncludeFields("price_*");

                                           
            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, requestBuilder.Build());
            var productType = AbstractTypeFactory<CatalogProduct>.TryCreateInstance().GetType();
            var binder = productType.GetIndexModelBinder();
            result.Result.Results = searchResult.Documents.Select(x => binder.BindModel(x, productType.GetBindingInfo())).OfType<CatalogProduct>().ToList(); 
            result.Result.TotalCount = (int)searchResult.TotalCount;
            return result;
        }
    }
}
