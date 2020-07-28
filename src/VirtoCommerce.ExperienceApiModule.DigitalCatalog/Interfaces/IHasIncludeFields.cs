using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Interfaces
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
