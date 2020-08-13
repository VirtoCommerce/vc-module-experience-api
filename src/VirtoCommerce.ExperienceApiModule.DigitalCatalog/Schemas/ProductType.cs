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
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;
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
        public ProductType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            Name = "Product";
            Description = "Products are the sellable goods in an e-commerce project.";

            Field(d => d.IndexedProduct.Id).Description("The unique ID of the product.");

            Field(d => d.IndexedProduct.Code, nullable: false).Description("The product SKU.");

            Field<StringGraphType>("catalogId", resolve: context => context.Source.IndexedProduct.CatalogId);

            FieldAsync<CategoryType>(
                "category",
                resolve: async context =>
                {
                    var categoryId = context.Source.IndexedProduct.CategoryId;
                    var responce = await mediator.Send(new LoadCategoryQuery
                    {
                        ObjectIds = new[] { categoryId },
                        IncludeFields = context.GetAllNodesPaths()
                    });

                    return responce.Category;
                })
            .RootAlias("__object.categoryId");

            Field(d => d.IndexedProduct.Name, nullable: false).Description("The name of the product.");

            Field<ListGraphType<DescriptionType>>(
                "descriptions",
                resolve: context => context.Source.IndexedProduct.Reviews)
            .RootAlias("__object.reviews");

            Field(d => d.IndexedProduct.ProductType, nullable: true).Description("The type of product");

            Field<StringGraphType>(
                "slug",
                description: "Get slug for product.",
                resolve: context =>
                {
                    //TODO: Need to refactor in future
                    var storeId = context.GetValue<string>("storeId");
                    var cultureName = context.GetValue<string>("cultureName");

                    return context.Source.IndexedProduct.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName)?.SemanticUrl;
                })
            .RootAlias("__object.seoInfos");

            Field<StringGraphType>(
                "metaDescription",
                description: "Get metaDescription for product.",
                resolve: context =>
                {
                    var storeId = context.GetValue<string>("storeId");
                    var cultureName = context.GetValue<string>("cultureName");

                    return context.Source.IndexedProduct.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName)?.MetaDescription;
                })
            .RootAlias("__object.seoInfos");

            Field<StringGraphType>(
                "metaKeywords",
                description: "Get metaKeywords for product.",
                resolve: context =>
                {
                    var storeId = context.GetValue<string>("storeId");
                    var cultureName = context.GetValue<string>("cultureName");

                    return context.Source.IndexedProduct.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName)?.MetaKeywords;
                })
            .RootAlias("__object.seoInfos");

            Field<StringGraphType>(
                "metaTitle",
                description: "Get metaTitle for product.",
                resolve: context =>
                {
                    var storeId = context.GetValue<string>("storeId");
                    var cultureName = context.GetValue<string>("cultureName");

                    return context.Source.IndexedProduct.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName)?.PageTitle;
                })
            .RootAlias("__object.seoInfos");

            Field<StringGraphType>(
                "imgSrc",
                description: "The product main image URL.",
                resolve: context => context.Source.IndexedProduct.ImgSrc)
            .InnerAlias("images");

            Field(d => d.IndexedProduct.OuterId, nullable: true).Description("The outer identifier");

            Field<StringGraphType>(
                "brandName",
                description: "Get brandName for product.",
                resolve: context =>
                {
                    var brandName = context.Source.IndexedProduct.Properties
                        ?.FirstOrDefault(x => x.Name == "Brand")
                        ?.Values
                        ?.FirstOrDefault(x => x.Value != null)
                        ?.Value;

                    return brandName;
                })
            .RootAlias("__object.properties");

            FieldAsync<VariationType>(
                "masterVariation",
                resolve: async context =>
                {
                    if (context.Source.IndexedProduct.MainProductId == null)
                    {
                        return new ExpVariation(context.Source);
                    }

                    var query = context.GetCatalogQuery<LoadProductsQuery>();
                    query.ObjectIds = new[] { context.Source.IndexedProduct.MainProductId };
                    query.IncludeFields = context.GetAllNodesPaths();

                    var response = await mediator.Send(query);

                    return response.Products
                        .Select(expProduct => new ExpVariation(expProduct))
                        .FirstOrDefault();
                })
            .RootAlias("__object.masterVariation", "__object.mainProductId", "__variations");

            FieldAsync<ListGraphType<VariationType>>(
                "variations",
                resolve: async context =>
                {
                    var productIds = context.Source.IndexedVariationIds.ToArray();
                    if (productIds.IsNullOrEmpty())
                    {
                        return new List<ExpVariation>();
                    }

                    var query = context.GetCatalogQuery<LoadProductsQuery>();
                    query.ObjectIds = context.Source.IndexedVariationIds.ToArray();
                    query.IncludeFields = context.GetAllNodesPaths();

                    var response = await mediator.Send(query);

                    return response.Products.Select(expProduct => new ExpVariation(expProduct));
                })
            .RootAlias("__variations");

            Field<AvailabilityDataType>(
                "availabilityData",
                resolve: context => new ExpAvailabilityData
                {
                    AvailableQuantity = context.Source.AvailableQuantity,
                    InventoryAll = context.Source.AllInventories,
                    IsBuyable = context.Source.IsBuyable,
                    IsAvailable = context.Source.IsAvailable,
                    IsInStock = context.Source.IsInStock,
                    IsActive = context.Source.IndexedProduct.IsActive ?? false,
                    IsTrackInventory = context.Source.IndexedProduct.TrackInventory ?? false,
                })
            .RootAlias("__object.isActive", "__object.isBuyable", "__object.trackInventory");

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.IndexedProduct.Images);

            Field<ListGraphType<PriceType>>(
                "prices",
                resolve: context => context.Source.AllPrices)
            .RootAlias("__prices");

            Field<ListGraphType<PropertyType>>("properties", resolve: context =>
            {
                var cultureName = context.GetValue<string>("cultureName");
                return context.Source.IndexedProduct.Properties.ExpandByValues(cultureName);
            });

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.IndexedProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.IndexedProduct.Outlines).RootAlias("__object.outlines");

            Field<ListGraphType<SeoInfoType>>("seoInfos", resolve: context => context.Source.IndexedProduct.SeoInfos);

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

        private static async Task<object> ResolveConnectionAsync(IMediator mediator, IResolveConnectionContext<ExpProduct> context)
        {
            var first = context.First;

            int.TryParse(context.After, out var skip);

            var query = new SearchProductAssociationsQuery
            {
                Skip = skip,
                Take = first ?? context.PageSize ?? 10,

                Keyword = context.GetArgument<string>("query"),
                Group = context.GetArgument<string>("group"),
                ObjectIds = new[] { context.Source.IndexedProduct.Id }
            };

            var response = await mediator.Send(query);

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
