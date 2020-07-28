using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class ProductType : ObjectGraphType<ExpProduct>
    {
        /// <example>
        ///{
        ///    product(id: "f1b26974b7634abaa0900e575a99476f")
        ///    {
        ///        id
        ///        code
        ///        category{ id code name hasParent slug }
        ///        name
        ///        metaTitle
        ///        metaDescription
        ///        metaKeywords
        ///        brandName
        ///        slug
        ///        imgSrc
        ///        productType
        ///        masterVariation {
        ///        images{ id url name }
        ///        assets{ id size url }
        ///        prices(cultureName: "en-us"){
        ///            list { amount }
        ///            currency
        ///        }
        ///        availabilityData{
        ///            availableQuantity
        ///            inventories{
        ///                inStockQuantity
        ///                fulfillmentCenterId
        ///                fulfillmentCenterName
        ///                allowPreorder
        ///                allowBackorder
        ///            }
        ///        }
        ///        properties{ id name valueType value valueId }
        ///    }
        ///}
        /// </example>
        public ProductType(IMediator mediator)
        {
            Name = "Product";
            Description = "Products are the sellable goods in an e-commerce project.";

            Field(d => d.CatalogProduct.Id).Description("The unique ID of the product.");

            Field(d => d.CatalogProduct.Code, nullable: false).Description("The product SKU.");

            Field<StringGraphType>("catalogId", resolve: context => context.Source.CatalogProduct.CatalogId);

            FieldAsync<CategoryType>("category", resolve: async context =>
            {
                var categoryId = context.Source.CatalogProduct.CategoryId;
                var responce = await mediator.Send(new LoadCategoryQuery
                {
                    Id = categoryId,
                    IncludeFields = context.SubFields.Values.GetAllNodesPaths()
                });

                return responce.Category;
            });

            Field(d => d.CatalogProduct.Name, nullable: false).Description("The name of the product.");

            Field<ListGraphType<DescriptionType>>("descriptions", resolve: context => context.Source.CatalogProduct.Reviews);

            Field(d => d.CatalogProduct.ProductType, nullable: true).Description("The type of product");

            Field<StringGraphType>(
                "slug",
                description: "Get slug for product.",
                resolve: context =>
                {
                    var semanticUrl = context.Source.CatalogProduct?.SeoInfos?.FirstOrDefault()?.SemanticUrl;
                    return semanticUrl;
                });

            Field<StringGraphType>(
                "metaDescription",
                description: "Get metaDescription for product.",
                resolve: context =>
                {
                    var metaDescription = context.Source.CatalogProduct?.SeoInfos?.FirstOrDefault()?.MetaDescription;
                    return metaDescription;
                });

            Field<StringGraphType>(
                "metaKeywords",
                description: "Get metaKeywords for product.",
                resolve: context =>
                {
                    var metaKeywords = context.Source.CatalogProduct?.SeoInfos?.FirstOrDefault()?.MetaKeywords;
                    return metaKeywords;
                });

            Field<StringGraphType>(
                "metaTitle",
                description: "Get metaTitle for product.",
                resolve: context =>
                {
                    var metaTitle = context.Source.CatalogProduct?.SeoInfos?.FirstOrDefault()?.PageTitle;
                    return metaTitle;
                });

            Field<StringGraphType>(
                "imgSrc",
                description: "The product main image URL.",
                resolve: context => context.Source.CatalogProduct.ImgSrc);

            Field(d => d.CatalogProduct.OuterId, nullable: true).Description("The outer identifier");

            Field<StringGraphType>(
                "brandName",
                description: "Get brandName for product.",
                resolve: context =>
                {
                    var brandName = context.Source.CatalogProduct.Properties
                        ?.FirstOrDefault(x => x.Name == "Brand")
                        ?.Values
                        ?.FirstOrDefault(x => x.Value != null)
                        ?.Value;

                    return brandName;
                });

            FieldAsync<VariationType>("masterVariation", resolve: async context =>
            {
                if (context.Source.CatalogProduct.MainProductId == null)
                {
                    var product = context.Source.CatalogProduct;

                    return new ExpVariation
                    {
                        Product = product,
                        Prices = context.Source.ProductPrices
                    };
                }

                var response = await mediator.Send(new LoadProductQuery
                {
                    Ids = new[] { context.Source.CatalogProduct.MainProductId },
                    IncludeFields = context.SubFields.Values.GetAllNodesPaths()
                });

                return response.Products.Select(expProduct => new ExpVariation
                {
                    Product = expProduct.CatalogProduct,
                    Prices = expProduct.ProductPrices
                }).FirstOrDefault();
            });

            FieldAsync<ListGraphType<VariationType>>("variations", resolve: async context =>
            {
                var variationsIds = context.Source.VariationIds;
                var response = await mediator.Send(new LoadProductQuery
                {
                    Ids = variationsIds.ToArray(),
                    IncludeFields = context.SubFields.Values.GetAllNodesPaths()
                });

                return response.Products.Select(expProduct => new ExpVariation
                {
                    Product = expProduct.CatalogProduct,
                    Prices = expProduct.ProductPrices
                });
            });

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.CatalogProduct.Properties);

            Field<TaxCategoryType>("tax", resolve: context => null); // TODO: We need this?

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

            int.TryParse(context.After, out var skip);

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
                    HasNextPage = response.Result.TotalCount > skip + first,
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.Result.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.Result.TotalCount,
            };
        }
    }
}
