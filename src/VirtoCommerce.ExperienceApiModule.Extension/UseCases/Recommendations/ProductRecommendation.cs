namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations
{
    public class ProductRecommendation
    {
        public string ProductId { get; set; }
        public string Scenario { get; set; }
        public string Type { get; set; }
        public decimal Score { get; set; }
    }
}
