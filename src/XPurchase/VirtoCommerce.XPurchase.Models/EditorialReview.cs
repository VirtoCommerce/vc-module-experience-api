using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models
{
    public partial class EditorialReview : LocalizedString, IAccessibleByIndexKey
    {
        public string ReviewType { get; set; }
        public string Content => Value;

        public string IndexKey => ReviewType;
    }
}
