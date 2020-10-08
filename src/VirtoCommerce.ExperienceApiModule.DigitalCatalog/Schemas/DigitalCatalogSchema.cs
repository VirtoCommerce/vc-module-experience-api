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
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class DigitalCatalogSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;

        public DigitalCatalogSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
        }

        /// <summary>
        /// XDigitalCatalog schema builder
        /// </summary>
        /// <remarks>
        /// IMPORTANT!
        /// We can't use the fluent syntax for new types registration provided by dotnet graphql here,
        /// because we have the strict requirement for underlying types extensions and must use
        /// GraphTypeExtenstionHelper to resolve the effective type on execution time
        /// </remarks>
        public void Build(ISchema schema)
        {
            var productField = new FieldType
            {
                Name = "product",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the product" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId", Description = "Store Id" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId", Description = "User Id" },
                    new QueryArgument<StringGraphType> { Name = "currencyCode", Description = "Currency code (\"USD\")" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-US\")" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    //TODO:  Need to check what there is no any alternative way to access to the original request arguments in sub selection
                    context.CopyArgumentsToUserContext();
                    var loader = _dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("productsLoader", (ids) => LoadProductsAsync(_mediator, ids, context));
                    return await loader.LoadAsync(context.GetArgument<string>("id"));
                })
            };
            schema.Query.AddField(productField);

            var productsConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ProductType, EdgeType<ProductType>, ProductsConnectonType<ProductType>, object>()
                .Name("products")
                .Argument<NonNullGraphType<StringGraphType>>("storeId", "The store id where products are searched")
                .Argument<NonNullGraphType<StringGraphType>>("userId", "The customer id for search result impersonalization")
                .Argument<StringGraphType>("currencyCode", "The currency for which all prices data will be returned")
                .Argument<StringGraphType>("cultureName", "The culture name for cart context product")
                .Argument<StringGraphType>("query", "The query parameter performs the full-text search")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<BooleanGraphType>("fuzzy", "When the fuzzy query parameter is set to true the search endpoint will also return products that contain slight differences to the search text.")
                .Argument<IntGraphType>("fuzzyLevel", "The fuzziness level is quantified in terms of the Damerau-Levenshtein distance, this distance being the number of operations needed to transform one word into another.")
                .Argument<StringGraphType>("facet", "Facets calculate statistical counts to aid in faceted navigation.")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Argument<ListGraphType<StringGraphType>>("productIds", "Product Ids") // TODO: make something good with it, move productIds in filter for example
                .Unidirectional()
                .PageSize(20);

            productsConnectionBuilder.ResolveAsync(async context =>
            {
                //TODO:  Need to check what there is no any alternative way to access to the original request arguments in sub selection
                context.CopyArgumentsToUserContext();
                return await ResolveProductsConnectionAsync(_mediator, context);
            });

            schema.Query.AddField(productsConnectionBuilder.FieldType);

            var categoriesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CategoryType, object>()
                .Name("categories")
                .Argument<NonNullGraphType<StringGraphType>>("storeId", "The store id where category are searched")
                .Argument<StringGraphType>("cultureName", "The language for which all localized category data will be returned")
                .Argument<NonNullGraphType<StringGraphType>>("userId", "The customer id for search result impersonalization")
                .Argument<StringGraphType>("currencyCode", "The currency for which all prices data will be returned")
                .Argument<StringGraphType>("query", "The query parameter performs the full-text search")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<BooleanGraphType>("fuzzy", "When the fuzzy query parameter is set to true the search endpoint will also return Categorys that contain slight differences to the search text.")
                .Argument<IntGraphType>("fuzzyLevel", "The fuzziness level is quantified in terms of the Damerau-Levenshtein distance, this distance being the number of operations needed to transform one word into another.")
                .Argument<StringGraphType>("facet", "Facets calculate statistical counts to aid in faceted navigation.")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Argument<ListGraphType<StringGraphType>>("categoryIds", "Category Ids") // TODO: make something good with it, move CategoryIds in filter for example
                .Unidirectional()
                .PageSize(20);

            categoriesConnectionBuilder.ResolveAsync(async context =>
            {
                //TODO:  Need to check what there is no any alternative way to access to the original request arguments in sub selection
                context.CopyArgumentsToUserContext();
                return await ResolveCategoriesConnectionAsync(_mediator, context);
            });

            schema.Query.AddField(categoriesConnectionBuilder.FieldType);
        }

        private static async Task<IDictionary<string, ExpProduct>> LoadProductsAsync(IMediator mediator, IEnumerable<string> ids, IResolveFieldContext context)
        {
            var query = context.GetCatalogQuery<LoadProductsQuery>();
            query.ObjectIds = ids.ToArray();
            query.IncludeFields = context.SubFields.Values.GetAllNodesPaths().Concat(new[] { "__object.id" }).ToArray();

            var response = await mediator.Send(query);

            return response.Products.ToDictionary(x => x.Id);
        }

        private static async Task<object> ResolveProductsConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var includeFields = context.SubFields.Values.GetAllNodesPaths();

            //TODO: Need to be able get entire query from context and read all arguments to the query properties
            var query = context.GetCatalogQuery<SearchProductQuery>();
            query.IncludeFields = includeFields;

            var productIds = context.GetArgument<List<string>>("productIds");
            if (productIds.IsNullOrEmpty())
            {
                query.Skip = skip;
                query.Take = first ?? context.PageSize ?? 10;
                query.Query = context.GetArgument<string>("query");
                query.Filter = context.GetArgument<string>("filter");
                query.Facet = context.GetArgument<string>("facet");
                query.Fuzzy = context.GetArgument<bool>("fuzzy");
                query.FuzzyLevel = context.GetArgument<int?>("fuzzyLevel");
                query.Sort = context.GetArgument<string>("sort");
            }
            else
            {
                query.ObjectIds = productIds.ToArray();
                query.Take = productIds.Count;
            }

            var response = await mediator.Send(query);

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
                    HasNextPage = response.TotalCount > skip + first,
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.TotalCount,
                Facets = response.Facets
            };
        }

        private static async Task<object> ResolveCategoriesConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var includeFields = context.SubFields.Values.GetAllNodesPaths().Select(x => x.Replace("items.", "")).ToArray();

            var query = context.GetCatalogQuery<SearchCategoryQuery>();

            var categoryIds = context.GetArgument<List<string>>("categoryIds");
            query.IncludeFields = includeFields;

            if (categoryIds.IsNullOrEmpty())
            {
                query.Skip = skip;
                query.Take = first ?? context.PageSize ?? 10;
                query.Query = context.GetArgument<string>("query");
                query.Filter = context.GetArgument<string>("filter");
                query.Facet = context.GetArgument<string>("facet");
                query.Fuzzy = context.GetArgument<bool>("fuzzy");
                query.FuzzyLevel = context.GetArgument<int?>("fuzzyLevel");
                query.Sort = context.GetArgument<string>("sort");
            }
            else
            {
                query.ObjectIds = categoryIds.ToArray();
                query.Take = categoryIds.Count;
            }

            var response = await mediator.Send(query);

            return new Connection<ExpCategory>()
            {
                Edges = response.Results
                    .Select((x, index) =>
                        new Edge<ExpCategory>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = x,
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = response.TotalCount > skip + first,
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.TotalCount
            };
        }
    }
}
