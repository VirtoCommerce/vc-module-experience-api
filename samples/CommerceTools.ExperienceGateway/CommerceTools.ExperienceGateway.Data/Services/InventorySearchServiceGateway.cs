using System;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{
    public class InventorySearchServiceGateway : IInventorySearchServiceGateway
    {
        public string Gateway { get; set; }

        public Task<InventoryInfoSearchResult> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}
