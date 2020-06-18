using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Catalog
{
    public class AggregationItemBreadcrumb : Breadcrumb
    {
        public AggregationItemBreadcrumb(AggregationItem item) : base("Tag")
        {
            AggregationItem = item;
        }

        public AggregationItem AggregationItem { get; private set; }
    }
}
