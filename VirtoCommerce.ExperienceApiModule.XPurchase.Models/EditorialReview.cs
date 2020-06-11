using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models
{
    public partial class EditorialReview : LocalizedString, IAccessibleByIndexKey
    {
        public string ReviewType { get; set; }
        public string Content => Value;

        public string IndexKey => ReviewType;
    }
}
