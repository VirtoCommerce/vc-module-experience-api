using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog
{
    public partial class Term : ValueObject
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
