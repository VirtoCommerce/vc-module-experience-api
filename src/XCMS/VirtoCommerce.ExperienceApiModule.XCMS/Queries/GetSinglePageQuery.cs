using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries;

public class GetSinglePageQuery : IQuery<PageItem>
{
    public string StoreId { get; set; }
    public string CultureName { get; set; }
    public string Id { get; set; }
}
