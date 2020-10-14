using System.Threading.Tasks;
using VirtoCommerce.TaxModule.Core.Model.Search;

namespace VirtoCommerce.XGateway.Core.Services
{
    public interface ITaxProviderSearchServiceGateway : IServiceGateway
    {
        Task<TaxProviderSearchResult> SearchTaxProvidersAsync(TaxProviderSearchCriteria criteria);
    }
}
