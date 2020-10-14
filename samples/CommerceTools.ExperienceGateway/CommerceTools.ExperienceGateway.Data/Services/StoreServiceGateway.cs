using System.Threading.Tasks;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XGateway.Core.Models;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{

    public class StoreServiceGateway : IStoreServiceGateway
    {
        public string Gateway { get; set; }

        public Task<Store> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
