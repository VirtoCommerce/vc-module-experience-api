using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class DigitalCatalogSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;
        private readonly ICurrencyService _currencyService;
        private readonly ICrudService<Store> _storeService;

        public DigitalCatalogSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader, ICurrencyService currencyService, IStoreService storeService)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
            _currencyService = currencyService;
            _storeService = (ICrudService<Store>)storeService;
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
                    new QueryArgument<StringGraphType> { Name = "userId", Description = "User Id" },
                    new QueryArgument<StringGraphType> { Name = "currencyCode", Description = "Currency code (\"USD\")" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-US\")" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<object, IDataLoaderResult<ExpProduct>>(async context =>
                {
                    //PT-1606:  Need to check that there is no any alternative way to access to the original request arguments in sub selection
                    context.CopyArgumentsToUserContext();

                    var store = await _storeService.GetByIdAsync(context.GetArgument<string>("storeId"));
                    context.UserContext["store"] = store;

                    var cultureName = context.GetArgument<string>("cultureName");

                    var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
                    //Store all currencies in the user context for future resolve in the schema types
                    context.SetCurrencies(allCurrencies, cultureName);

                    var loader = _dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>("productsLoader", (ids) => LoadProductsAsync(_mediator, ids, context));
                    return loader.LoadAsync(context.GetArgument<string>("id"));
                })
            };
            schema.Query.AddField(productField);
            var categoryField = new FieldType
            {
                Name = "category",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the product" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId", Description = "Store Id" },
                    new QueryArgument<StringGraphType> { Name = "userId", Description = "User Id" },
                    new QueryArgument<StringGraphType> { Name = "currencyCode", Description = "Currency code (\"USD\")" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "Culture name (\"en-US\")" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<CategoryType>(),
                Resolver = new AsyncFieldResolver<ExpCategory, IDataLoaderResult<ExpCategory>>(async context =>
               {
                   var store = await _storeService.GetByIdAsync(context.GetArgument<string>("storeId"));
                   context.UserContext["store"] = store;

                   //PT-1606:  Need to check that there is no any alternative way to access to the original request arguments in sub selection
                   context.CopyArgumentsToUserContext();

                   var loader = _dataLoader.Context.GetOrAddBatchLoader<string, ExpCategory>("categoriesLoader", (ids) => LoadCategoriesAsync(_mediator, ids, context));
                   return loader.LoadAsync(context.GetArgument<string>("id"));
               })
            };
            schema.Query.AddField(categoryField);

            var categoriesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<CategoryType, object>()
                .Name("categories")
                .Argument<StringGraphType>("storeId", "The store id where category are searched")
                .Argument<StringGraphType>("cultureName", "The language for which all localized category data will be returned")
                .Argument<StringGraphType>("userId", "The customer id for search result impersonation")
                .Argument<StringGraphType>("currencyCode", "The currency for which all prices data will be returned")
                .Argument<StringGraphType>("query", "The query parameter performs the full-text search")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<BooleanGraphType>("fuzzy", "When the fuzzy query parameter is set to true the search endpoint will also return categories that contain slight differences to the search text.")
                .Argument<IntGraphType>("fuzzyLevel", "The fuzziness level is quantified in terms of the Damerau-Levenshtein distance, this distance being the number of operations needed to transform one word into another.")
                .Argument<StringGraphType>("facet", "Facets calculate statistical counts to aid in faceted navigation.")
                .Argument<StringGraphType>("sort", "The sort expression")
                .Argument<ListGraphType<StringGraphType>>("categoryIds", "Category Ids")
                .PageSize(20);

            categoriesConnectionBuilder.ResolveAsync(async context =>
            {
                var store = await _storeService.GetByIdAsync(context.GetArgument<string>("storeId"));
                context.UserContext["store"] = store;

                //PT-1606:  Need to check that there is no any alternative way to access to the original request arguments in sub selection
                context.CopyArgumentsToUserContext();
                return await ResolveCategoriesConnectionAsync(_mediator, context);
            });

            schema.Query.AddField(categoriesConnectionBuilder.FieldType);

            var propertiesConnectionBuilder = GraphTypeExtenstionHelper.CreateConnection<PropertyType, object>()
                .Name("properties")
                .Argument<NonNullGraphType<StringGraphType>>("storeId", "The store id to get associated catalog")
                .Argument<ListGraphType<PropertyTypeEnum>>("types", "The owner types (Catalog, Category, Product, Variation)")
                .Argument<StringGraphType>("filter", "This parameter applies a filter to the query results")
                .Argument<StringGraphType>("cultureName", "The language for which all localized property dictionary items will be returned")
                .PageSize(20);

            propertiesConnectionBuilder.ResolveAsync(async context =>
            {
                var store = await _storeService.GetByIdAsync(context.GetArgument<string>("storeId"));
                context.UserContext["catalog"] = store.Catalog;

                //PT-1606:  Need to check that there is no any alternative way to access to the original request arguments in sub selection
                context.CopyArgumentsToUserContext();
                return await ResolvePropertiesConnectionAsync(_mediator, context);
            });

            schema.Query.AddField(propertiesConnectionBuilder.FieldType);

            var propertyField = new FieldType
            {
                Name = "property",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the property" },
                    new QueryArgument<StringGraphType> { Name = "cultureName", Description = "The language for which all localized property dictionary items will be returned" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<PropertyType>(),
                Resolver = new AsyncFieldResolver<PropertyType, IDataLoaderResult<Property>>(context =>
                {
                    //PT-1606:  Need to check that there is no any alternative way to access to the original request arguments in sub selection
                    context.CopyArgumentsToUserContext();
                    var loader = _dataLoader.Context.GetOrAddBatchLoader<string, Property>("propertiesLoader", (ids) => LoadPropertiesAsync(_mediator, ids));
                    return Task.FromResult(loader.LoadAsync(context.GetArgument<string>("id")));
                })
            };
            schema.Query.AddField(propertyField);
        }

        private static async Task<IDictionary<string, ExpProduct>> LoadProductsAsync(IMediator mediator, IEnumerable<string> ids, IResolveFieldContext context)
        {
            var query = context.GetCatalogQuery<LoadProductsQuery>();
            query.ObjectIds = ids.ToArray();
            query.IncludeFields = context.SubFields.Values.GetAllNodesPaths(context).ToArray();

            var response = await mediator.Send(query);

            return response.Products.ToDictionary(x => x.Id);
        }

        private static async Task<IDictionary<string, ExpCategory>> LoadCategoriesAsync(IMediator mediator, IEnumerable<string> ids, IResolveFieldContext context)
        {
            var query = context.GetCatalogQuery<LoadCategoryQuery>();
            query.ObjectIds = ids.ToArray();
            query.IncludeFields = context.SubFields.Values.GetAllNodesPaths(context).ToArray();

            var response = await mediator.Send(query);

            return response.Categories.ToDictionary(x => x.Id);
        }

        protected virtual async Task<IDictionary<string, Property>> LoadPropertiesAsync(IMediator mediator, IEnumerable<string> ids)
        {
            var result = await mediator.Send(new LoadPropertiesQuery { Ids = ids });

            return result.Properties;
        }

        private static async Task<object> ResolveCategoriesConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var includeFields = context.SubFields.Values.GetAllNodesPaths(context).Select(x => x.Replace("items.", "")).ToArray();

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

            return new PagedConnection<ExpCategory>(response.Results, query.Skip, query.Take, response.TotalCount);
        }

        private static async Task<object> ResolvePropertiesConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
        {
            var first = context.First;

            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var query = new SearchPropertiesQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,

                CatalogId = (string)context.UserContext["catalog"],
                Types = context.GetArgument<object[]>("types"),
                Filter = context.GetArgument<string>("filter")
            };

            var response = await mediator.Send(query);

            return new PagedConnection<Property>(response.Result.Results, query.Skip, query.Take, response.Result.TotalCount);
        }
    }
}
