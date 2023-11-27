using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ChildCategoriesQueryResponse
{
    public IList<ExpCategory> ChildCategories { get; set; } = new List<ExpCategory>();
}
