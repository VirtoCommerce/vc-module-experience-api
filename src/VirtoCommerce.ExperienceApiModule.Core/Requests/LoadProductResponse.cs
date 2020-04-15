using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public class LoadProductResponse
    {
        public ICollection<CatalogProduct> Products { get; set; } = new List<CatalogProduct>();
    }
}
