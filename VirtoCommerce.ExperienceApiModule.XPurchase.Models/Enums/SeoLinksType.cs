namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums
{
    public enum SeoLinksType
    {
        /// <summary>
        /// /category/123
        /// /product/123
        /// </summary>
        None,

        /// <summary>
        /// /my-cool-category
        /// /my-cool-product
        /// </summary>
        Short,

        /// <summary>
        /// /virtual-parent-category/physical-parent-category/my-cool-category
        /// /virtual-parent-category/physical-parent-category/my-cool-category/my-cool-product
        /// </summary>
        Long,

        /// <summary>
        /// virtual-parent-category/my-cool-category
        /// virtual-parent-category/my-cool-category/my-cool-product
        /// </summary>
        Collapsed,
    }
}
