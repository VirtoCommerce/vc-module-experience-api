using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IExpPricingService
    {
        Task<ProductPrice[]> EvaluateProductPricesAsync(SearchProductResponse parameter, PricingModule.Core.Model.PriceEvaluationContext priceEvaluationContext);
    }
}
