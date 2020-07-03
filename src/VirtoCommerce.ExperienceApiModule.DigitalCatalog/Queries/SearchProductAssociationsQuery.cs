using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries
{
    public class SearchProductAssociationsQuery : IQuery<SearchProductAssociationsResponse>
    {
        public ProductAssociationSearchCriteria Criteria { get; set; }
    }
}
