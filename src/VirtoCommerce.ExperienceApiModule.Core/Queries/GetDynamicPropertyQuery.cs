using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetDynamicPropertyQuery : IDynamicPropertiesQuery, IQuery<GetDynamicPropertyResponse>
    {
        public string IdOrName { get; set; }
        public string CultureName { get; set; }
        public string ObjectType { get; set; }
    }
}
