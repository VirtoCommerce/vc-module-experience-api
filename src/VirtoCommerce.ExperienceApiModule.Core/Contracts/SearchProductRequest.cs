using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.Core.Contracts
{
    public class SearchProductRequest : IRequest<SearchProductResponse>
    {
        public ProductSearchCriteria Criteria { get; set; }
    }
}
