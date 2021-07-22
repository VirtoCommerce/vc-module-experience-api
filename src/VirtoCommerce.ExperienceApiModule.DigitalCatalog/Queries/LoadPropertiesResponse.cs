using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPropertiesResponse
    {
        public IDictionary<string, Property> Properties { get; set; }
    }
}
