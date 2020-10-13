using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class StoreServiceVirtoCommerce : IStoreServiceGateway
    {
        private readonly IStoreService _storeService;

        public StoreServiceVirtoCommerce(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public string Gateway { get; set; } = ExperienceApiModule.Core.Models.Gateways.VirtoCommerce;

        public Task<Store> GetByIdAsync(string id)
        {
            return _storeService.GetByIdAsync(id);
        }
    }
}
