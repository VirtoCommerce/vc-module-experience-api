using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
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
            Field(d => d.IndexedProduct.ProductType, nullable: true).Description("The type of product");

            FieldAsync<StringGraphType>("outline", resolve: async context =>
            {
                var outlines = context.Source.IndexedProduct.Outlines;
                if (outlines.IsNullOrEmpty()) return null;

                var loadRelatedCatalogOutlineQuery = context.GetCatalogQuery<LoadRelatedCatalogOutlineQuery>();
                loadRelatedCatalogOutlineQuery.Outlines = outlines;

                var response = await mediator.Send(loadRelatedCatalogOutlineQuery);
                return response.Outline;
            }, description: @"All parent categories ids relative to the requested catalog and concatenated with \ . E.g. (1/21/344)");

            FieldAsync<StringGraphType>("slug", resolve: async context =>
            {
                var outlines = context.Source.IndexedProduct.Outlines;
                if (outlines.IsNullOrEmpty()) return null;

                var loadRelatedSlugPathQuery = context.GetCatalogQuery<LoadRelatedSlugPathQuery>();
                loadRelatedSlugPathQuery.Outlines = outlines;

                var response = await mediator.Send(loadRelatedSlugPathQuery);
                return response.Slug;
            }, description: "Request related slug for product");

            Field(d => d.IndexedProduct.Name, nullable: false).Description("The name of the product.");

            Field<SeoInfoType>("seoInfo", resolve: context =>
            {
                var source = context.Source;
                var storeId = context.GetArgumentOrValue<string>("storeId");
                var cultureName = context.GetArgumentOrValue<string>("cultureName");

                SeoInfo seoInfo = null;

                if (!source.IndexedProduct.SeoInfos.IsNullOrEmpty())
                {
                    seoInfo = source.IndexedProduct.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName);
                }

                return seoInfo ?? new SeoInfo
                {
                    SemanticUrl = source.Id,
                    LanguageCode = cultureName,
                    Name = source.IndexedProduct.Name
                };
            }, description: "Request related SEO info");

            Field<ListGraphType<DescriptionType>>("descriptions", resolve: context => context.Source.IndexedProduct.Reviews);

            Field<DescriptionType>("description", resolve: context =>
            {
                var reviews = context.Source.IndexedProduct.Reviews;
                var cultureName = context.GetArgumentOrValue<string>("cultureName");

                if (!reviews.IsNullOrEmpty())
                {
                    return reviews.Where(x => x.ReviewType.EqualsInvariant("FullReview")).FirstBestMatchForLanguage(cultureName) as EditorialReview
                        ?? reviews.FirstBestMatchForLanguage(cultureName) as EditorialReview;
                }

                return null;
            });

            FieldAsync<CategoryType>(
                "category",
                resolve: async context =>
                {
                    var categoryId = context.Source.IndexedProduct.CategoryId;

                    var loadCategoryQuery = context.GetCatalogQuery<LoadCategoryQuery>();
                    loadCategoryQuery.ObjectIds = new[] { categoryId };
                    loadCategoryQuery.IncludeFields = context.SubFields.Values.GetAllNodesPaths();

                    var responce = await mediator.Send(loadCategoryQuery);

                    return responce.Categories.FirstOrDefault();
                });

            Field<StringGraphType>(
                "imgSrc",
                description: "The product main image URL.",
                resolve: context => context.Source.IndexedProduct.ImgSrc);

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
                });

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
                    query.IncludeFields = context.SubFields.Values.GetAllNodesPaths();

                    var response = await mediator.Send(query);

                    return response.Products.Select(expProduct => new ExpVariation(expProduct));
                });

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
                });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.IndexedProduct.Images);

            Field<PriceType>(
                "price",
                resolve: context => context.Source.AllPrices.FirstOrDefault());

            Field<ListGraphType<PriceType>>(
                "prices",
                resolve: context => context.Source.AllPrices);

            Field<ListGraphType<PropertyType>>("properties", resolve: context =>
            {
                var cultureName = context.GetValue<string>("cultureName");
                return context.Source.IndexedProduct.Properties.ExpandByValues(cultureName);
            });

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.IndexedProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.IndexedProduct.Outlines);//.RootAlias("__object.outlines");

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

            return new PagedConnection<ProductAssociation>(response.Result.Results, skip, Convert.ToInt32(context.After ?? 0.ToString()), response.Result.TotalCount);
        }
    }
}
