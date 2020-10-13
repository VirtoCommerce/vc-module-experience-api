using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class ProductAssociationSearchServiceCommerceTools : IProductAssociationSearchServiceGateway
    {
        public ProductAssociationSearchServiceCommerceTools()
        {

        }
        public string Gateway { get; set; } = Gateways.CommerceTools;

        public Task<SearchProductAssociationsResponse> SearchProductAssociationsAsync(SearchProductAssociationsQuery request)
        {
            throw new NotImplementedException();
        }
    }
}
