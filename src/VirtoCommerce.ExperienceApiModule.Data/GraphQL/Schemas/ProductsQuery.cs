using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class ProductsQuery : ObjectGraphType<object>
    {
        public ProductsQuery(IItemService productService, IProductSearchService productSearchService, IDataLoaderContextAccessor dataLoader)
        {
            Name = "Query";

            FieldAsync<ProductType>(
                 "product",
                 arguments: new QueryArguments(
                     new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the product" }
                 ),
                resolve: async context =>
                {
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, CatalogProduct>("productsLoader", (ids) => LoadProductsAsync(productService, ids));

                    return await loader.LoadAsync(context.GetArgument<string>("id"));
                }
             );

            Connection<ProductType>()
                .Name("products")
                .Argument<ListGraphType<StringGraphType>>("query", "the search phrase")
                .Argument<ListGraphType<StringGraphType>>("catalog", "the catalog id")
                .Unidirectional()
                .PageSize(20)
                .ResolveAsync(async context =>
                {
                    return await ResolveConnectionAsync(productSearchService, context);
                });

        }

        public static async Task<IDictionary<string, CatalogProduct>> LoadProductsAsync(IItemService productService, IEnumerable<string> ids)
        {
            var products = await productService.GetByIdsAsync(ids.ToArray(), ItemResponseGroup.ItemInfo.ToString());
            return products.ToDictionary(x => x.Id);
        }


        private static async Task<object> ResolveConnectionAsync(IProductSearchService productSearchService, IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var criteria = new ProductSearchCriteria
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                ResponseGroup = ItemResponseGroup.ItemInfo.ToString(),
                Keyword = context.GetArgument<string>("query"),
                CatalogId = context.GetArgument<string>("catalog"),
            };
            var searchResult = await productSearchService.SearchProductsAsync(criteria);


            return new Connection<CatalogProduct>()
            {
                Edges = searchResult.Results
                    .Select((x, index) =>
                        new Edge<CatalogProduct>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = x,
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = searchResult.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = (skip + first).ToString(),
                },
                TotalCount = searchResult.TotalCount,
            };
        }

    }
}
