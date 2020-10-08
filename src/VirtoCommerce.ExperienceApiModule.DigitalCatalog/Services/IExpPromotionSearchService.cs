using System.Threading.Tasks;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IExpPromotionSearchService
    {
        Task<LoadPromotionsResponse> SearchPromotionsAsync(LoadPromotionsQuery request);
    }
}
