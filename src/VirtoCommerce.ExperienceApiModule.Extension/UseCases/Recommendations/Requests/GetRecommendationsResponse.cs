using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Requests
{
    public class GetRecommendationsResponse : DownstreamResponse
    {
        public IList<ProductRecommendation> Products { get; set; } = new List<ProductRecommendation>();
        public int TotalCount { get; set; }
        protected override Task InitializeAsync(DownstreamRequest request)
        {
            var jTokens = Json.SelectTokens(request.Scenario.Connection.ResponseJsonPath, errorWhenNoMatch: true);
            foreach (var jToken in jTokens)
            {
                var product = new ProductRecommendation
                {
                    ProductId = jToken.ToString(),
                    Scenario = request.Scenario.Name,
                };
                Products.Add(product);
            }
            return Task.CompletedTask;
        }
    }
}
