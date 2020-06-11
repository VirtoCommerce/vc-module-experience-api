using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog
{
    public class ProductBreadcrumb : Breadcrumb
    {
        public ProductBreadcrumb(Product product) : base(nameof(Product))
        {
            Product = product;
        }
        public Product Product { get; private set; }
    }
}
