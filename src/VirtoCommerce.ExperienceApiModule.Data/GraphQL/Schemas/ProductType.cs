using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Relay.Types;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using GraphQL.Authorization;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.CatalogModule.Core.Search;
using GraphQL.Builders;
using System;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using GraphQL.Types.Relay.DataObjects;
using VirtoCommerce.ExperienceApiModule.Core;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Requests;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class ProductType : ObjectGraphType<CatalogProduct>
    {
        public ProductType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Name = "Product";
            Description = "Products are the sellable goods in an e-commerce project.";

            //this.AuthorizeWith(CatalogModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(d => d.Id).Description("The unique ID of the product.");
            Field(d => d.Name, nullable: false).Description("The name of the product.");
            Field(d => d.ProductType, nullable: true).Description("The type of product");
            Field(d => d.Code, nullable: false).Description("The product SKU.");
            Field(d => d.ImgSrc).Description("The product main image URL.");
            Field(d => d.OuterId).Description("The outer identifier");
            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.Properties);
          

            //FieldAsync<ListGraphType<PropertyType>>(
            //    "properties",
            //     arguments: new QueryArguments(
            //         new QueryArgument<StringGraphType> { Name = "type", Description = "the type of properties" }
            //    ),
            //    resolve: async context =>
            //    {
            //        var propType = context.GetArgument<string>("type");
            //        var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<string, Property>($"propertyLoader{propType}", (ids) => LoadProductsPropertiesAsync(mediator, ids, propType));

            //        // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
            //        var loadHandle = loader.LoadAsync(context.Source.Id);
            //        return await loadHandle;
            //    }
            //);

            Connection<ProductAssociationType>()
              .Name("associations")
              .Argument<StringGraphType>("query", "the search phrase")
              .Argument<StringGraphType>("group", "association group (Accessories, RelatedItem)")
              .Unidirectional()
              .PageSize(20)
              .ResolveAsync(async context =>
              {
                  return await ResolveConnectionAsync(mediator, context);
              });
        }

        private static async Task<object> ResolveConnectionAsync(IMediator madiator, IResolveConnectionContext<CatalogProduct> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var criteria = new ProductAssociationSearchCriteria
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                ResponseGroup = ItemResponseGroup.ItemInfo.ToString(),
                Keyword = context.GetArgument<string>("query"),
                Group = context.GetArgument<string>("group"),
                ObjectIds = new[] { context.Source.Id }
            };
            var response = await madiator.Send(new SearchProductAssociationsRequest { Criteria = criteria });
            return new Connection<ProductAssociation>()
            {
                Edges = response.Result.Results
                    .Select((x, index) =>
                        new Edge<ProductAssociation>()
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

        public static async Task<ILookup<string, Property>> LoadProductsPropertiesAsync(IMediator mediator, IEnumerable<string> ids, string type = null)
        {
            var response = await mediator.Send(new LoadProductRequest { Ids = ids.ToArray(), ResponseGroup = ItemResponseGroup.ItemProperties.ToString() });
            return response.Products.SelectMany(x => x.Properties.Where(x=> type == null || x.Type.ToString().EqualsInvariant(type))
                           .Select(p => new { ProductId = x.Id, Property = p }))
                           .ToLookup(x => x.ProductId, x => x.Property);
        }      
    }
}
