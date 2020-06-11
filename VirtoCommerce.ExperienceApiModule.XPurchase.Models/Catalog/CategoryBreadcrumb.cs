using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog
{
    public class CategoryBreadcrumb : Breadcrumb
    {
        public CategoryBreadcrumb(Category category) : base(nameof(Category)) => Category = category;

        public Category Category { get; private set; }
    }
}
