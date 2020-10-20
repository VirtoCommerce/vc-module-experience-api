using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class GetProductRatingQuery : IQuery<ProductRating>
    {
        public string ProductId { get; set; }
    }
}
