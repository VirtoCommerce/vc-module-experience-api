using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IPricingServiceGateway : IServiceGateway
    {
        Task<ProductPrice[]> EvaluateProductPricesAsync(SearchProductResponse parameter, PricingModule.Core.Model.PriceEvaluationContext priceEvaluationContext);
    }
}
