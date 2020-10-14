using System;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{
    public class ProductAssociationSearchServiceGateway : IProductAssociationSearchServiceGateway
    {
        public string Gateway { get; set; }

        public Task<ProductAssociationSearchResult> SearchProductAssociationsAsync(ProductAssociationSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}
