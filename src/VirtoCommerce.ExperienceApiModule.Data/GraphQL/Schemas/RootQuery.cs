using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Contracts;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class RootQuery : ObjectGraphType<object>
    {

        public RootQuery(
              IMediator mediator
            , IDataLoaderContextAccessor dataLoader)
        {
            Name = "RootQuery";

            FieldAsync<ProductType>(
                 "product",
                 arguments: new QueryArguments(
                     new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the product" }
                 ),
                resolve: async context =>
                {
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, CatalogProduct>("productsLoader", (ids) => LoadProductsAsync(mediator, ids));

                    return await loader.LoadAsync(context.GetArgument<string>("id"));
                }
             );

            Connection<ProductType>()
                .Name("products")
                .Argument<StringGraphType>("query", "the search phrase")
                .Argument<StringGraphType>("catalog", "the catalog id")
                .Unidirectional()
                .PageSize(20)
                .ResolveAsync(async context =>
                {
                    return await ResolveConnectionAsync(mediator, context);
                });

        }

        public static async Task<IDictionary<string, CatalogProduct>> LoadProductsAsync(IMediator mediator, IEnumerable<string> ids)
        {
            var response = await mediator.Send(new LoadProductRequest { Ids = ids.ToArray(), ResponseGroup = ItemResponseGroup.ItemInfo.ToString() });
            return response.Products.ToDictionary(x => x.Id);
        }


        private static async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
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
            var response = await mediator.Send(new SearchProductRequest { Criteria = criteria });


            return new Connection<CatalogProduct>()
            {
                Edges = response.Result.Results
                    .Select((x, index) =>
                        new Edge<CatalogProduct>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = x,
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = response.Result.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.Result.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.Result.TotalCount,
            };
        }

    }
}
