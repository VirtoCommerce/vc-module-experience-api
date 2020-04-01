using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    public class CatalogProduct2 : CatalogProduct
    {
        public decimal? Price { get; set; }
        public string Currency { get; set; }
    }
}
