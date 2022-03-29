using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CoreSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;

        public CoreSchema(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Build(ISchema schema)
        {
            #region countries query

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     countries
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion

            _ = schema.Query.AddField(new FieldType
            {
                Name = "countries",
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<CountryType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetCountriesQuery());

                    return result.Countries;
                })
            });

            #region regions query

#pragma warning disable S125 // Sections of code should not be commented out
            /*                         
               query {
                     regions(countryId: "country code")
               }                         
            */
#pragma warning restore S125 // Sections of code should not be commented out

            #endregion

            _ = schema.Query.AddField(new FieldType
            {
                Name = "regions",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "countryId" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<CountryRegionType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetRegionsQuery
                    {
                        CountryId = context.GetArgument<string>("countryId"),
                    });

                    return result.Regions;
                })
            });

#pragma warning disable S125 // Sections of code should not be commented out
            
        }
    }
}
