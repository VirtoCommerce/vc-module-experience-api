using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetDynamicPropertyQuery : IDynamicPropertiesQuery, IQuery<GetDynamicPropertyResponse>
    {
        public string CultureName { get; set; }
    }
}
