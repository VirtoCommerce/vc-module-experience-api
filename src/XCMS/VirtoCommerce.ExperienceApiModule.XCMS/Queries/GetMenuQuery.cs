using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetMenuQuery : IQuery<GetMenuResponse>
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string Name { get; set; }
    }
}
