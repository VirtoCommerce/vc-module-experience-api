using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ProductSuggestionsQueryResponse
{
    public IList<string> Suggestions { get; set; } = new List<string>();
}
