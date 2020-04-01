using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.Core.Contracts
{
    public class SearchProductAssociationsRequest : IRequest<SearchProductAssociationsResponse>
    {
        public ProductAssociationSearchCriteria Criteria { get; set; }
    }
}
