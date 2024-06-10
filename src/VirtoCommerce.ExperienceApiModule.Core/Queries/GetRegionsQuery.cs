using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetRegionsQuery : IQuery<GetRegionsResponse>
    {
        public string CountryId { get; set; }
    }
}
