using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CountryType : ObjectGraphType<Country>
    {
        public CountryType(IMediator mediator)
        {
            Field(x => x.Id);
            Field(x => x.Name);
            FieldAsync<ListGraphType<CountryRegionType>>("regions", resolve: async (x) =>
           {
               var response = await mediator.Send(new GetRegionsQuery() { CountryId = x.Source.Id });
               return response.Regions;
           });
        }
    }
}
