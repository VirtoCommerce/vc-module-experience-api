using System.Threading.Tasks;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XGateway.Core.Services;

namespace VirtoCommerce.ExperienceGatewayModule.Data.Services
{
    public class TaxProviderSearchServiceGateway : ITaxProviderSearchServiceGateway
    {
        private readonly ITaxProviderSearchService _taxProviderSearchService;
        public TaxProviderSearchServiceGateway(/*ITaxProviderSearchService taxProviderSearchService*/)
        {
            //_taxProviderSearchService = taxProviderSearchService;
        }

        public Task<TaxProviderSearchResult> SearchTaxProvidersAsync(TaxProviderSearchCriteria criteria)
        {
            return _taxProviderSearchService.SearchTaxProvidersAsync(criteria);
        }
    }
}
