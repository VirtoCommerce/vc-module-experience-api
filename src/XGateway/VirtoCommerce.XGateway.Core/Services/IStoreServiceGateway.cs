using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XGateway.Core.Services
{
    public interface IStoreServiceGateway : IServiceGateway
    {
        Task<Store> GetByIdAsync(string id);
    }
}
