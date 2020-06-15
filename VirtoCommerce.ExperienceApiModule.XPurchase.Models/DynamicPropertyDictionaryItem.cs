using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models
{
    public partial class DynamicPropertyDictionaryItem : Entity
    {
        public string PropertyId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IList<LocalizedString> DisplayNames { get; set; } = new List<LocalizedString>();
    }
}
