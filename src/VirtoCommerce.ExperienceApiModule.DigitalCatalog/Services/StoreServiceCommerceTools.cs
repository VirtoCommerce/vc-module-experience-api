using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Services
{

    public class StoreServiceCommerceTools : IStoreServiceGateway
    {
        public string Gateway { get; set; } = Gateways.CommerceTools;

        public Task<Store> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
