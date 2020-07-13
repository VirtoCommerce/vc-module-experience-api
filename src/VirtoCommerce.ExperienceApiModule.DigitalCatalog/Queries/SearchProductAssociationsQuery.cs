using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchProductAssociationsQuery : IQuery<SearchProductAssociationsResponse>
    {
        public ProductAssociationSearchCriteria Criteria { get; set; }
    }
}
