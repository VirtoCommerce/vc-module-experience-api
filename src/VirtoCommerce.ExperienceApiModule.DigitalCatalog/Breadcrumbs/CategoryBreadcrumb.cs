using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Breadcrumbs
{
    public class CategoryBreadcrumb : Breadcrumb
    {
        public CategoryBreadcrumb(Category category) : base(nameof(Category))
        {
            Category = category;
        }

        public Category Category { get; private set; }
    }
}
