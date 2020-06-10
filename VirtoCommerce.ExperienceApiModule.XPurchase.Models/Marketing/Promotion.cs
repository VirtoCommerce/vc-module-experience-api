using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing
{
    /// <summary>
    /// Represents promotion object.
    /// </summary>
    public partial class Promotion : CloneableEntity
    {
        public IList<string> Coupons { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
