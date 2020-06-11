using System.Collections.Specialized;
using System.Runtime.Serialization;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart
{
    public partial class CartSearchCriteria : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public CartSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }

        public CartSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public string Sort { get; set; }

        public string StoreId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        [IgnoreDataMember]
        public User Customer { get; set; }

        [IgnoreDataMember]
        public Currency Currency { get; set; }

        [IgnoreDataMember]
        public Language Language { get; set; }
    }
}
