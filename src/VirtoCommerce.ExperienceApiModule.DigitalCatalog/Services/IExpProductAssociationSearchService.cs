using System.Threading.Tasks;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public interface IExpProductAssociationSearchService
    {
        Task<SearchProductAssociationsResponse> SearchProductAssociationsAsync(SearchProductAssociationsQuery request);
    }
}
