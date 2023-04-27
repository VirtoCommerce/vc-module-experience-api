using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetPageQuery : IQuery<GetPageResponse>
    {
        public string StoreId { get; set; }
        public string Keyword { get; set; }
    }
}
