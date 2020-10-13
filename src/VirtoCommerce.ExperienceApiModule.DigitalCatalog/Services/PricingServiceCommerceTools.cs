using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class PricingServiceCommerceTools : IPricingServiceGateway
    {
        public string Gateway { get; set; } = Gateways.CommerceTools;

        public Task<ExperienceApiModule.Core.Models.ProductPrice[]> EvaluateProductPricesAsync(SearchProductResponse parameter, PriceEvaluationContext priceEvaluationContext)
        {
            throw new NotImplementedException();
        }
    }
}
