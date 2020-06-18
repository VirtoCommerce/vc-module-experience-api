using VirtoCommerce.XPurchase.Models.Catalog;

namespace VirtoCommerce.XPurchase.Models.Customer
{
    public partial class Vendor : Member
    {
        public string Description { get; set; }

        public string SiteUrl { get; set; }

        public string LogoUrl { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// Vendor seo info
        /// </summary>
        public SeoInfo SeoInfo { get; set; }

        public IMutablePagedList<Product> Products { get; set; }

        public string Handle => SeoInfo?.Slug ?? Id;
    }
}
