using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IStoreServiceGateway
    {
        Task<Store> GetByIdAsync(string id);
    }
}
