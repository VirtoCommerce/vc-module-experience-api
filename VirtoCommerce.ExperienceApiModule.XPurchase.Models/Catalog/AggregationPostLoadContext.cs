using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog
{
    public class AggregationPostLoadContext
    {
        public ProductSearchCriteria ProductSearchCriteria { get; set; }
        public IDictionary<string, Category> CategoryByIdDict { get; set; }
    }
}
