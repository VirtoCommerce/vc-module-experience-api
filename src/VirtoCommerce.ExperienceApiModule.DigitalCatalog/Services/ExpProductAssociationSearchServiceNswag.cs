using System;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Services
{
    public class ExpProductAssociationSearchServiceNswag : IExpProductAssociationSearchService, IService
    {
        public ExpProductAssociationSearchServiceNswag()
        {

        }
        public string Provider { get; set; } = Providers.Nswag;

        public Task<SearchProductAssociationsResponse> SearchProductAssociationsAsync(SearchProductAssociationsQuery request)
        {
            throw new NotImplementedException();
        }
    }
}
