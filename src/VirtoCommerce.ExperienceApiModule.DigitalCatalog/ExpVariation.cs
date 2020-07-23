using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpVariation
    {
        public CatalogProduct Product { get; set; }
        public List<ProductPrice> Prices { get; set; }
    }
}
