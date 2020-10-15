using System;
using System.Threading.Tasks;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.XGateway.Core.Services;

namespace CommerceTools.ExperienceGateway.Data.Services
{
    public class TaxProviderSearchServiceGateway : ITaxProviderSearchServiceGateway
    {
        public Task<TaxProviderSearchResult> SearchTaxProvidersAsync(TaxProviderSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}
