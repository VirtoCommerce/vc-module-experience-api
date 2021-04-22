using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.Platform.Core.DynamicProperties;
namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DynamicPropertySchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;

        public DynamicPropertySchema(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Build(ISchema schema)
        {
            var dynamicPropertyField = new FieldType
            {
                Name = "dynamicProperty",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "idOrName", Description = "Id or name of the dynamic property" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-US\")" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<DynamicPropertyType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    context.CopyArgumentsToUserContext();

                    var query = context.GetDynamicPropertiesQuery<GetDynamicPropertyQuery>();
                    query.IdOrName = context.GetArgument<string>("idOrName");

                    var response = await _mediator.Send(query);

                    return response.DynamicProperty;
                })
            };
            schema.Query.AddField(dynamicPropertyField);

            var dynamicPropertiesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<DynamicPropertyType, object>()
                .Name("dynamicProperties")
                .Argument<StringGraphType>("cultureName", "The culture name (\"en-US\")")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Unidirectional()
                .PageSize(20);

            dynamicPropertiesConnectionBuilder.ResolveAsync(async context => await ResolveDynamicPropertiesConnectionAsync(_mediator, context));

            schema.Query.AddField(dynamicPropertiesConnectionBuilder.FieldType);
        }

        private static async Task<object> ResolveDynamicPropertiesConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            int.TryParse(context.After, out var skip);

            var query = context.GetDynamicPropertiesQuery<SearchDynamicPropertiesQuery>();
            query.Skip = skip;
            query.Take = context.First ?? context.PageSize ?? 10;
            query.Sort = context.GetArgument<string>("sort");
            query.Filter = context.GetArgument<string>("filter");

            context.CopyArgumentsToUserContext();

            var response = await mediator.Send(query);

            return new PagedConnection<DynamicProperty>(response.Results, skip, Convert.ToInt32(context.After ?? 0.ToString()), response.TotalCount);
        }
    }
}
