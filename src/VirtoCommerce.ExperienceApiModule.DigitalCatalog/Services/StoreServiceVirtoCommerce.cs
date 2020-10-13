using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class StoreServiceVirtoCommerce : IStoreServiceGateway, IService
    {
        private readonly IStoreService _storeService;

        public StoreServiceVirtoCommerce(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public string Provider { get; set; } = Providers.PlatformModule;

        public Task<Store> GetByIdAsync(string id)
        {
            return _storeService.GetByIdAsync(id);
        }
    }
}
