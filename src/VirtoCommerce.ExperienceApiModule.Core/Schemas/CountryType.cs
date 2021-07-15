using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CountryType : ObjectGraphType<Country>
    {
        public CountryType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field<ListGraphType<CountryRegionType>>("regions", resolve: x => x.Source.Regions);
        }
    }
}
