using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Types.Relay;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class ProductQuery : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;
        public ProductQuery(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
        }
        public void Build(ISchema schema)
        {
            var productField = new FieldType
            {
                Name = "product",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the product" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    
                    var loader = _dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("productsLoader", (ids) => LoadProductsAsync(_mediator, ids, context.SubFields.Values.GetAllNodesPaths()));
                    return await loader.LoadAsync(context.GetArgument<string>("id"));
                })
            };
            schema.Query.AddField(productField);

            //var productsConnectionBuilder = ConnectionBuilder.Create<ProductType, EdgeType<ProductType>, ProductsConnectonType<ProductType>, object>()
            var productsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ProductType, EdgeType<ProductType>, ProductsConnectonType<ProductType>, object>()
                .Name("products")
                .Argument<StringGraphType>("query", "The query parameter performs the full-text search")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<BooleanGraphType>("fuzzy", "When the fuzzy query parameter is set to true the search endpoint will also return products that contain slight differences to the search text.")
                .Argument<StringGraphType>("facet", "Facets calculate statistical counts to aid in faceted navigation.")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Unidirectional()
                .PageSize(20);

            productsConnectionBuilder.ResolveAsync(async context =>
                {
                    return await ResolveConnectionAsync(_mediator, context);
                });


            schema.Query.AddField(productsConnectionBuilder.FieldType);

        }

        public static async Task<IDictionary<string, ExpProduct>> LoadProductsAsync(IMediator mediator, IEnumerable<string> ids, IEnumerable<string> includeFields)
        {
            var response = await mediator.Send(new LoadProductRequest { Ids = ids.ToArray(), IncludeFields = includeFields });
            return response.Products.ToDictionary(x => x.Id);
        }


        private static async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var includeFields = context.SubFields.Values.GetAllNodesPaths().Select(x => x.TrimStart("items.")).ToArray();
            var request = new SearchProductRequest
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                Query = context.GetArgument<string>("query"),
                Filter = context.GetArgument<string>("filter"),
                Facet = context.GetArgument<string>("facet"),
                Fuzzy = context.GetArgument<bool>("fuzzy"),
                Sort = context.GetArgument<string>("sort"),
                IncludeFields = includeFields.ToArray(),
            };

            var response = await mediator.Send(request);


            return new ProductsConnection<ExpProduct>()
            {
                Edges = response.Results
                    .Select((x, index) =>
                        new Edge<ExpProduct>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = x,
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = response.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.TotalCount,
                Facets = response.Facets
            };
        }
    }
}
