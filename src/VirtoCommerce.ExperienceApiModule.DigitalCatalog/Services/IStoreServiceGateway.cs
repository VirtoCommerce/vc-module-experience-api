using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IStoreServiceGateway : IServiceGateway
    {
        Task<Store> GetByIdAsync(string id);
    }
}
