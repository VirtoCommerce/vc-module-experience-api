using System.Collections.Generic;

namespace VirtoCommerce.XPurchase.Models.Common
{
    public interface IHasBreadcrumbs
    {
        IEnumerable<Breadcrumb> GetBreadcrumbs();
    }
}
