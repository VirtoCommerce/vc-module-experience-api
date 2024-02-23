using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XPurchase.Queries
{
    public partial class GetWishlistQuery : IQuery<CartAggregate>
    {
        public string ListId { get; set; }

        public string CultureName { get; set; }

        public IList<string> IncludeFields { get; set; }
    }
}
