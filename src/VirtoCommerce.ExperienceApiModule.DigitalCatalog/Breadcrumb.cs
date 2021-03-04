using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog
{
    public abstract class Breadcrumb
    {
        protected Breadcrumb(string type)
        {
            TypeName = type;
        }
        public string TypeName { get; private set; }
        public virtual string Title { get; set; }
        public virtual string SeoPath { get; set; }
        public virtual string Url { get; set; }
    }

    public class CategoryBreadcrumb : Breadcrumb
    {
        public CategoryBreadcrumb(Category category) : base(nameof(Category))
        {
            Category = category;
        }

        public Category Category { get; private set; }
    }

    public class ProductBreadcrumb : Breadcrumb
    {
        public ProductBreadcrumb(CatalogProduct product) : base(nameof(Product))
        {
            Product = product;
        }
        public CatalogProduct Product { get; private set; }
    }
}
