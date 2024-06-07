using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Core.Models;

public class ProductSuggestionsQueryResponse
{
    public IList<string> Suggestions { get; set; } = new List<string>();
}
