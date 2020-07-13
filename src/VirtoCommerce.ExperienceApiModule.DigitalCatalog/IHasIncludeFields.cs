using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
