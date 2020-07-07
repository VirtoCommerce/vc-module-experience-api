using System;
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
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class ProductType : ObjectGraphType<ExpProduct>
    {
        public ProductType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Name = "Product";
            Description = "Products are the sellable goods in an e-commerce project.";

            //this.AuthorizeWith(CatalogModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(d => d.CatalogProduct.Id).Description("The unique ID of the product.");
            Field(d => d.CatalogProduct.Name, nullable: false).Description("The name of the product.");
            Field(d => d.CatalogProduct.ProductType, nullable: true).Description("The type of product");
            Field(d => d.CatalogProduct.Code, nullable: false).Description("The product SKU.");
            Field(d => d.CatalogProduct.ImgSrc).Description("The product main image URL.");
            Field(d => d.CatalogProduct.OuterId, nullable: true).Description("The outer identifier");
            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.CatalogProduct.Properties);

            Field<ListGraphType<PriceType>>("prices", arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "currency", Description = "currency" }), resolve: context =>
            {
                var result = (context.Source).Prices;
                var currency = context.GetArgument<string>("currency");
                if (currency != null)
                {
                    result = result.Where(x => x.Currency.EqualsInvariant(currency)).ToList();
                }
                return result;
            });

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

        private static async Task<object> ResolveConnectionAsync(IMediator madiator, IResolveConnectionContext<ExpProduct> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());
            var criteria = new ProductAssociationSearchCriteria
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,
                // We control the resulting product structure  by passing IncludeFields, and to prevent forced reduction of already loaded fields, you need to pass ItemResponseGroup.Full
                // in any case, the object will be loaded from the index, and the response group will not affect overall performance
                ResponseGroup = ItemResponseGroup.Full.ToString(),
                Keyword = context.GetArgument<string>("query"),
                Group = context.GetArgument<string>("group"),
                ObjectIds = new[] { context.Source.CatalogProduct.Id }
            };
            var response = await madiator.Send(new SearchProductAssociationsQuery { Criteria = criteria });
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
    }
}
