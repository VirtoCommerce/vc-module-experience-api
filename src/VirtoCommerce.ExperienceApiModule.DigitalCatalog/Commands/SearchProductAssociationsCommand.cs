using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests
{
    public class SearchProductAssociationsCommand : IRequest<SearchProductAssociationsResponse>
    {
        public ProductAssociationSearchCriteria Criteria { get; set; }
    }
}
