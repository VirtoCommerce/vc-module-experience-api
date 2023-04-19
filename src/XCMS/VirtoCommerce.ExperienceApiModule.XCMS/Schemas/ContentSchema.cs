using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XCMS.Queries;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Schemas
{
    public class ContentSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;

        public ContentSchema(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Build(ISchema schema)
        {
            _ = schema.Query.AddField(new FieldType
            {
                Name = "menus",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                    new QueryArgument<StringGraphType> { Name = "cultureName" },
                    new QueryArgument<StringGraphType> { Name = "keyword" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<ListGraphType<MenuLinkListType>>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetMenusQuery
                    {
                        StoreId = context.GetArgument<string>("storeId"),
                        CultureName = context.GetArgument<string>("cultureName"),
                        Keyword = context.GetArgument<string>("keyword"),
                    });

                    return result.Menus;
                })
            });

            _ = schema.Query.AddField(new FieldType
            {
                Name = "menu",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cultureName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<MenuLinkListType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetMenuQuery
                    {
                        StoreId = context.GetArgument<string>("storeId"),
                        CultureName = context.GetArgument<string>("cultureName"),
                        Name = context.GetArgument<string>("name"),
                    });

                    return result.MenuList;
                })
            });
        }
    }
}
