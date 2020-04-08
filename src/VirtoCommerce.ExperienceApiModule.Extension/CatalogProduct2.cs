using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Extension
{
    public class CatalogProduct2 : CatalogProduct
    {
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public string DataSource { get; set; }
    }
}
