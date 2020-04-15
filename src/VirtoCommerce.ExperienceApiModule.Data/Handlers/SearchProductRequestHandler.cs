using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nest;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class SearchProductRequestHandler : IRequestHandler<SearchProductRequest, SearchProductResponse>
    {
        private readonly ElasticClient _client;
        public SearchProductRequestHandler()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("default-product");
            _client = new ElasticClient(settings);
        }

        public virtual async Task<SearchProductResponse> Handle(SearchProductRequest request, CancellationToken cancellationToken)
        {
            var response = new SearchProductResponse();
            var searchResponse = _client.Search<SearchDocument>(s => s.From(request.Criteria.Skip).Size(request.Criteria.Take).Source(src => src.Includes(i => i.Fields(request.IncludeFields.Select(x => "product_src." + x).ToArray()))));
            foreach (var doc in searchResponse.Documents)
            {
                var jsonString = JsonConvert.SerializeObject(doc["product_src"]);
                var product = JsonConvert.DeserializeObject(jsonString, AbstractTypeFactory<CatalogProduct>.TryCreateInstance().GetType()) as CatalogProduct;
                response.Result.Results.Add(product);
                product.OuterId = "Virto";
            }
            response.Result.TotalCount = (int)searchResponse.Total;
            return response;


        }
    }
}
