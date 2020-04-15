using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.SourceFilteringTest.Tests
{
    public class SourceFilteringTest
    {
        public class SearchDocument : Dictionary<string, object>
        {
            public SearchDocument()
            {

            }

            public string Id { get; set; }
        }

        [Fact]
        public Task DoSomeSearch()
        {
             var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("default-product");

            var client = new ElasticClient(settings);
            var searchResponse = client.Search<SearchDocument>(s => s.From(0)
                                                                    .Size(10)
                                                                    .Source(src => src.Includes(i => i.Field("product_src"))));
            var products = new List<CatalogProduct>();
            foreach (var doc in searchResponse.Documents)
            {

                var jsonString = JsonConvert.SerializeObject(doc["product_src"]);
                var product = JsonConvert.DeserializeObject<CatalogProduct>(jsonString);
                products.Add(product);

            }
           
            return Task.CompletedTask;
        }

    }
}
