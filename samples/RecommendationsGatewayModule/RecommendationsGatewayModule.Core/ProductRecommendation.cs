namespace RecommendationsGatewayModule.Core
{
    public class ProductRecommendation
    {
        public string ProductId { get; set; }
        public string Scenario { get; set; }
        public decimal Score { get; set; }
    }
}
