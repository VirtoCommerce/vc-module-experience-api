using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IPromotionSearchServiceGateway : IServiceGateway
    {
        Task<LoadPromotionsResponse> SearchPromotionsAsync(LoadPromotionsQuery request);
    }
}
