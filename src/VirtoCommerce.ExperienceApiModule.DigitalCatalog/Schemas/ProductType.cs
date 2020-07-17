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
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using GraphQL.Instrumentation;
using GraphQL.Language.AST;
using System.Collections.Generic;
using System.Reflection;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class ExpDescription
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }

    public class DescriptionType : ObjectGraphType<ExpDescription>
    {
        public DescriptionType()
        {
            Field(x => x.Type).Description("Description type.");
            Field(x => x.Text).Description("Description text.");
        }
    }

    public class ProductType : ObjectGraphType<ExpProduct>
    {
        /// <summary>
        ///
        /// </summary>
        /// <example>
        ///
        /// </example>
        public ProductType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Name = "Product";
            Description = "Products are the sellable goods in an e-commerce project.";

            //this.AuthorizeWith(CatalogModule.Core.ModuleConstants.Security.Permissions.Read);

            Field(d => d.CatalogProduct.Id).Description("The unique ID of the product.");
            Field(d => d.CatalogProduct.Code, nullable: false).Description("The product SKU.");

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

            FieldAsync<ListGraphType<DescriptionType>>("descriptions", resolve: async context =>
            {
                var descriptions = await mediator.Send(new object());

                return descriptions;
            });

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

            // TODO: resolve this correctly
            Field<VariationType>("masterVariation", resolve: context =>
            {
                var variation = new Variation();
                variation = Map(variation, context.Source.CatalogProduct);
                return variation;
            });

            Field<ListGraphType<VariationType>>("variations", resolve: context => context.Source.CatalogProduct.Variations);

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.CatalogProduct.Properties);

            Field<ListGraphType<PriceType>>(
                "prices",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "currency", Description = "currency" }),
                resolve: context =>
                {
                    var result = context.Source.Prices;
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

        // TODO: move this to extensions if this needed in future :p
        private static T Map<T, TU>(T target, TU source)
        {
            typeof(T)
                .GetTypeInfo()
                .GetProperties()
                .Where(x => x.CanWrite)
                .ToList()
                .ForEach(prop =>
                {
                    var sp = source.GetType().GetProperty(prop.Name);
                    if (sp != null)
                    {
                        var value = sp.GetValue(source, null);
                        target.GetType().GetProperty(prop.Name).SetValue(target, value, null);
                    }
                });

            return target;
        }
    }
}
