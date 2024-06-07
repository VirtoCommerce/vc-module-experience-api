using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Core.Models;

public class ChildCategoriesQueryResponse
{
    public IList<ExpCategory> ChildCategories { get; set; }
}
