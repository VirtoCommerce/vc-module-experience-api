using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Catalog
{
    public class CategoryBreadcrumb : Breadcrumb
    {
        public CategoryBreadcrumb(Category category) : base(nameof(Category)) => Category = category;

        public Category Category { get; private set; }
    }
}
