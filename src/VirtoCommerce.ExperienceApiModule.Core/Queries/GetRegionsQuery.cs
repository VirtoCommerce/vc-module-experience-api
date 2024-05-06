using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetRegionsQuery : IQuery<GetRegionsResponse>
    {
        public string CountryId { get; set; }
    }
}
