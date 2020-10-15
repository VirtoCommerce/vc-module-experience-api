using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class StoreServiceGateway : IStoreServiceGateway
    {
        private readonly IStoreService _storeService;

        public StoreServiceGateway(/*IStoreService storeService*/)
        {
            //_storeService = storeService;
        }

        public Task<Store> GetByIdAsync(string id)
        {
            return Task.FromResult(new Store());
            //return _storeService.GetByIdAsync(id);
        }
    }
}
