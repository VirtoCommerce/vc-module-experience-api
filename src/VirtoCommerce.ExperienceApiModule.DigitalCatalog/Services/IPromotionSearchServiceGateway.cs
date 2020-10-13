using System.Threading.Tasks;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IPromotionSearchServiceGateway
    {
        Task<LoadPromotionsResponse> SearchPromotionsAsync(LoadPromotionsQuery request);
    }
}
