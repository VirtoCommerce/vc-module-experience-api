using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nest;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class LoadProductsRequestHandler : IRequestHandler<LoadProductRequest, LoadProductResponse>
    {
        private readonly ElasticClient _client;
        public LoadProductsRequestHandler()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("default-product");
            _client = new ElasticClient(settings);
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductRequest request, CancellationToken cancellationToken)
        {
            var result = new LoadProductResponse();
            var searchResponse = _client.Search<SearchDocument>(s => s.Query(q => q.Ids(c => c.Values(request.Ids))).Source(src => src.Includes(i => i.Fields(request.IncludeFields.Select(x=> "product_src." + x).ToArray()))));
            foreach (var doc in searchResponse.Documents)
            {
                var jsonString = JsonConvert.SerializeObject(doc["product_src"]);
                var product = JsonConvert.DeserializeObject(jsonString, AbstractTypeFactory<CatalogProduct>.TryCreateInstance().GetType()) as CatalogProduct;
                result.Products.Add(product);
                product.OuterId = "Virto";
            }
            return result;
        }
    }
}
