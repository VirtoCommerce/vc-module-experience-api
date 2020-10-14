using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class StoreServiceGateway : IStoreServiceGateway
    {
        private readonly IStoreService _storeService;

        public StoreServiceGateway(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public string Gateway { get; set; } = Gateways.VirtoCommerce;

        public Task<Store> GetByIdAsync(string id)
        {
            return _storeService.GetByIdAsync(id);
        }
    }
}
