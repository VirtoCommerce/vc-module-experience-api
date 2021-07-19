using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchPropertiesQuery : CatalogQueryBase<SearchPropertiesResponse>
    {
        public string CatalogId { get; set; }
        public object[] Types { get; set; }
        public string Keyword { get; set; }
        public string CategoryId { get; set; }
        public IList<string> PropertyNames { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        
    }
}
