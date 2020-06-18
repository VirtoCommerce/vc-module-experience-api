using VirtoCommerce.XPurchase.Models.Common;

namespace VirtoCommerce.XPurchase.Models.Catalog
{
    public partial class Term : ValueObject
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
