using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{
    public class PricingServiceGateway : IPricingServiceGateway
    {
        public Task<IEnumerable<Price>> EvaluateProductPricesAsync(PriceEvaluationContext evalContext)
        {
            throw new NotImplementedException();
        }
    }
}
