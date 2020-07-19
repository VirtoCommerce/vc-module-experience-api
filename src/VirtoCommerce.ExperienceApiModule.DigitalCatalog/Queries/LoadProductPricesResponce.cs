using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductPricesResponce
    {
        public IEnumerable<ProductPrice> ProductPrices { get; set; }
    }
}
