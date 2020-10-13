using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class InventorySearchServiceCommerceTools : IInventorySearchServiceGateway
    {
        public string Gateway { get; set; } = Gateways.CommerceTools;

        public Task<SearchProductResponse> SearchInventoriesAsync(SearchProductResponse query)
        {
            throw new NotImplementedException();
        }
    }
}
