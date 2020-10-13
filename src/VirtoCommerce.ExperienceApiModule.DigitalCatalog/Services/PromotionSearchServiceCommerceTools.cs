using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class PromotionSearchServiceCommerceTools : IPromotionSearchServiceGateway
    {
        public string Gateway { get; set; } = Gateways.CommerceTools;

        public Task<LoadPromotionsResponse> SearchPromotionsAsync(LoadPromotionsQuery request)
        {
            throw new NotImplementedException();
        }
    }
}
