using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XCMS.Core.Models;

namespace VirtoCommerce.XCMS.Core.Queries
{
    public class GetMenuQuery : IQuery<GetMenuResponse>
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string Name { get; set; }
    }
}
