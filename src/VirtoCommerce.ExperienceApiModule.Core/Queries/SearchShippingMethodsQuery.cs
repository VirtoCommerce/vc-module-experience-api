using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ShippingModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SearchShippingMethodsQuery : IQuery<ShippingMethodsSearchResult>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string StoreId { get; set; }
    }
}
