using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class PricingServiceGateway : IPricingServiceGateway
    {
        private readonly IPricingService _pricingService;

        public PricingServiceGateway(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public Task<IEnumerable<Price>> EvaluateProductPricesAsync(PriceEvaluationContext priceEvaluationContext)
        {
            return _pricingService.EvaluateProductPricesAsync(priceEvaluationContext);
        }
    }
}
