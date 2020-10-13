using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class PricingServiceVirtoCommerce : IPricingServiceGateway
    {
        private readonly IPricingService _pricingService;
        private readonly IMapper _mapper;

        public PricingServiceVirtoCommerce(IPricingService pricingService, IMapper mapper)
        {
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public async Task<ExperienceApiModule.Core.Models.ProductPrice[]> EvaluateProductPricesAsync(SearchProductResponse parameter, PriceEvaluationContext evalContext)
        {
            var prices = await _pricingService.EvaluateProductPricesAsync(evalContext);
            var result = _mapper.Map<ExperienceApiModule.Core.Models.ProductPrice[]>(prices, options =>
            {
                //TODO: Code duplication
                options.Items["all_currencies"] = parameter.AllStoreCurrencies;
                options.Items["currency"] = parameter.Currency;
            });

            return result;
        }
    }
}
