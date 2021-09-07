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
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class ProductType : ExtendableGraphType<ExpProduct>
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

                return seoInfo ?? GetFallbackSeoInfo(source, cultureName);
            }, description: "Request related SEO info");

            Field<ListGraphType<DescriptionType>>("descriptions",
                  arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "type" }),
                  resolve: context =>
                {
                    var reviews = context.Source.IndexedProduct.Reviews;
                    var cultureName = context.GetArgumentOrValue<string>("cultureName");
                    var type = context.GetArgumentOrValue<string>("type");
                    if (cultureName != null)
                    {
                        reviews = reviews.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(cultureName)).ToList();
                    }
                    if (type != null)
                    {
                        reviews = reviews.Where(x => x.ReviewType?.EqualsInvariant(type) ?? true).ToList();
                    }
                    return reviews;
                });

            Field<DescriptionType>("description",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "type" }),
                resolve: context =>
            {
                var reviews = context.Source.IndexedProduct.Reviews;
                var type = context.GetArgumentOrValue<string>("type");
                var cultureName = context.GetArgumentOrValue<string>("cultureName");

                if (!reviews.IsNullOrEmpty())
                {
                    return reviews.Where(x => x.ReviewType.EqualsInvariant(type ?? "FullReview")).FirstBestMatchForLanguage(cultureName) as EditorialReview
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

            Field(
                GraphTypeExtenstionHelper.GetActualType<AvailabilityDataType>(),
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

            Field<ListGraphType<ImageType>>(
                "images",
                resolve: context =>
                {
                    var images = context.Source.IndexedProduct.Images;

                    return context.GetValue<string>("cultureName") switch
                    {
                        // Get images with null or current cultureName value if cultureName is passed
                        string languageCode => images.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(languageCode)).ToList(),

                        // CultureName is null
                        _ => images
                    };
                });

            Field<PriceType>(
                "price",
                resolve: context => context.Source.AllPrices.FirstOrDefault() ?? new ProductPrice(context.GetCurrencyByCode(context.GetValue<string>("currencyCode"))));

            Field<ListGraphType<PriceType>>(
                "prices",
                resolve: context => context.Source.AllPrices);

            ExtendableField<ListGraphType<PropertyType>>("properties",
                arguments: new QueryArguments(new QueryArgument<ListGraphType<StringGraphType>> { Name = "names" }),
                resolve: context =>
            {
                var names = context.GetArgument<string[]>("names");
                var cultureName = context.GetValue<string>("cultureName");
                var result = context.Source.IndexedProduct.Properties.ExpandByValues(cultureName);
                if (!names.IsNullOrEmpty())
                {
                    result = result.Where(x => names.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
                }
                return result;
            });

            Field<ListGraphType<AssetType>>(
                "assets",
                resolve: context =>
                {
                    var assets = context.Source.IndexedProduct.Assets;

                    return context.GetValue<string>("cultureName") switch
                    {
                        // Get assets with null or current cultureName value if cultureName is passed
                        string languageCode => assets.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(languageCode)).ToList(),

                        // CultureName is null
                        _ => assets
                    };
                });

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.IndexedProduct.Outlines);//.RootAlias("__object.outlines");

            Field<ListGraphType<BreadcrumbType>>("breadcrumbs", resolve: context =>
            {
                var store = context.GetArgumentOrValue<Store>("store");
                var cultureName = context.GetValue<string>("cultureName");

                return context.Source.IndexedProduct.Outlines.GetBreadcrumbsFromOutLine(store, cultureName);
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

        private static SeoInfo GetFallbackSeoInfo(ExpProduct source, string cultureName)
        {
            var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
            result.SemanticUrl = source.Id;
            result.LanguageCode = cultureName;
            result.Name = source.IndexedProduct.Name;
            return result;
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

            return new PagedConnection<ProductAssociation>(response.Result.Results, query.Skip, query.Take, response.Result.TotalCount);
        }
    }
}
