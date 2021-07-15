using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CountryRegionType : ObjectGraphType<CountryRegion>
    {
        public CountryRegionType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
        }
    }
}
