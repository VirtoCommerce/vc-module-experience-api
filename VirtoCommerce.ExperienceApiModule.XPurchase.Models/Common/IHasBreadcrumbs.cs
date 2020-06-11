using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public interface IHasBreadcrumbs
    {
        IEnumerable<Breadcrumb> GetBreadcrumbs();
    }
}
