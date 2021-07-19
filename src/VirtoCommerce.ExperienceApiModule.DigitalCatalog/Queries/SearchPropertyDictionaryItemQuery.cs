using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchPropertyDictionaryItemQuery : CatalogQueryBase<SearchPropertyDictionaryItemResponse>
    {
        public int Skip { get; set; }
        public int Take { get; set; }

        public IList<string> PropertyIds { get; set; }
    }
}
