using System.Collections.Generic;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class SearchPropertyDictionaryItemQuery : CatalogQueryBase<SearchPropertyDictionaryItemResponse>
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public IList<string> PropertyIds { get; set; }
    }
}
