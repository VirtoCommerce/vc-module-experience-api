using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetMenusQuery : IQuery<GetMenusResponse>
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string Keyword { get; set; }
    }
}
