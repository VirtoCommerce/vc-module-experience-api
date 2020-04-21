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
    public class LoadProductsRequestHandler : IRequestHandler<LoadProductRequest, LoadProductResponse>
    {
        private readonly ISearchProvider _searchProvider;
 
        public LoadProductsRequestHandler(ISearchProvider searchProvider)
        {
            _searchProvider = searchProvider;
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductRequest request, CancellationToken cancellationToken)
        {
            var result = new LoadProductResponse();
            var searchRequest = new SearchRequestBuilder()
                                            .WithPaging(0, request.Ids.Count())
                                            .WithIncludeFields(request.IncludeFields)
                                            .AddObjectIds(request.Ids)
                                            .Build();

            var searchResult = await _searchProvider.SearchAsync(KnownDocumentTypes.Product, searchRequest);
            result.Products = searchResult.Documents.Select(x=>x.Materialize<CatalogProduct>()).ToList();

           return result;
        }
    }
}
