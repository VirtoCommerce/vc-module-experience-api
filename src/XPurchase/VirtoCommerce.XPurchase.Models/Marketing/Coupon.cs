using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Marketing
{
    /// <summary>
    /// Represents coupon object
    /// </summary>
    public partial class Coupon : ValueObject
    {
        /// <summary>
        /// Gets or sets coupon code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets coupon description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the sign that coupon was applied successfully
        /// </summary>
        public bool AppliedSuccessfully { get; set; }

        /// <summary>
        /// Gets or sets coupon error code
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
