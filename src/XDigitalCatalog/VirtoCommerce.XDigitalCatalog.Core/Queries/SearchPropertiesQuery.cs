using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries
{
    public class SearchPropertiesQuery : CatalogQueryBase<SearchPropertiesResponse>
    {
        public string CatalogId { get; set; }
        public object[] Types { get; set; }
        public string Filter { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

    }
}
