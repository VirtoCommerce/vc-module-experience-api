using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Breadcrumbs
{
    public class ProductBreadcrumb : Breadcrumb
    {
        public ProductBreadcrumb(CatalogProduct product) : base(nameof(Product))
        {
            Product = product;
        }
        public CatalogProduct Product { get; private set; }
    }
}
