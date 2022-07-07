using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CategoryType : ExtendableGraphType<ExpCategory>
    {
        public CategoryType(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            Name = "Category";

            Field(x => x.Id, nullable: false).Description("Id of category.");
            Field(x => x.Category.ImgSrc, nullable: true).Description("The category image.");
            Field(x => x.Category.Code, nullable: false).Description("SKU of category.");
            Field(x => x.Category.Name, nullable: false).Description("Name of category.");
            Field(x => x.Level, nullable: true).Description(@"Level in hierarchy");

            FieldAsync<StringGraphType>("outline", resolve: async context =>
             {
                 var outlines = context.Source.Category.Outlines;
                 if (outlines.IsNullOrEmpty()) return null;

                 var loadRelatedCatalogOutlineQuery = context.GetCatalogQuery<LoadRelatedCatalogOutlineQuery>();
                 loadRelatedCatalogOutlineQuery.Outlines = outlines;

                 var response = await mediator.Send(loadRelatedCatalogOutlineQuery);
                 return response.Outline;
             }, description: @"All parent categories ids relative to the requested catalog and concatenated with \ . E.g. (1/21/344)");

            FieldAsync<StringGraphType>("slug", resolve: async context =>
            {
                var outlines = context.Source.Category.Outlines;
                if (outlines.IsNullOrEmpty()) return null;

                var loadRelatedSlugPathQuery = context.GetCatalogQuery<LoadRelatedSlugPathQuery>();
                loadRelatedSlugPathQuery.Outlines = outlines;

                var response = await mediator.Send(loadRelatedSlugPathQuery);
                return response.Slug;
            }, description: @"Request related slug for category");

            Field(x => x.Category.Path, nullable: true).Description("Category path in to the requested catalog  (all parent categories names concatenated. E.g. (parent1/parent2))");

            Field<SeoInfoType>("seoInfo", resolve: context =>
            {
                var source = context.Source;
                var storeId = context.GetArgumentOrValue<string>("storeId");
                var cultureName = context.GetArgumentOrValue<string>("cultureName");

                SeoInfo seoInfo = null;

                if (!source.Category.SeoInfos.IsNullOrEmpty())
                {
                    seoInfo = source.Category.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName);
                }

                return seoInfo ?? GetFallbackSeoInfo(source, cultureName);
            }, description: "Request related SEO info");

            Field<ListGraphType<CategoryDescriptionType>>("descriptions",
                  arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "type" }),
                  resolve: context =>
                  {
                      var descriptions = context.Source.Category.Descriptions;
                      var cultureName = context.GetArgumentOrValue<string>("cultureName");
                      var type = context.GetArgumentOrValue<string>("type");
                      if (cultureName != null)
                      {
                          descriptions = descriptions.Where(x => string.IsNullOrEmpty(x.LanguageCode) || x.LanguageCode.EqualsInvariant(cultureName)).ToList();
                      }
                      if (type != null)
                      {
                          descriptions = descriptions.Where(x => x.DescriptionType?.EqualsInvariant(type) ?? true).ToList();
                      }
                      return descriptions;
                  });

            Field<CategoryDescriptionType>("description",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "type" }),
                resolve: context =>
                {
                    var descriptions = context.Source.Category.Descriptions;
                    var type = context.GetArgumentOrValue<string>("type");
                    var cultureName = context.GetArgumentOrValue<string>("cultureName");

                    if (!descriptions.IsNullOrEmpty())
                    {
                        return descriptions.Where(x => x.DescriptionType.EqualsInvariant(type ?? "FullReview")).FirstBestMatchForLanguage(cultureName) as CategoryDescription
                            ?? descriptions.FirstBestMatchForLanguage(cultureName) as CategoryDescription;
                    }

                    return null;
                });

            Field<CategoryType, ExpCategory>("parent").ResolveAsync(ctx =>
            {
                var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpCategory>("parentsCategoryLoader", (ids) => LoadCategoriesAsync(mediator, ids, ctx));
                if (TryGetCategoryParentId(ctx, out var parentCategoryId))
                {
                    return loader.LoadAsync(parentCategoryId);
                }
                return null;
            });

            Field<BooleanGraphType>("hasParent",
                "Have a parent",
                resolve: context => TryGetCategoryParentId(context, out _));
            Field<ListGraphType<OutlineType>>("outlines",
                "Outlines",
                resolve: context => context.Source.Category.Outlines);
            Field<ListGraphType<ImageType>>("images",
                "Images",
                resolve: context => context.Source.Category.Images);
            Field<ListGraphType<BreadcrumbType>>("breadcrumbs",
                "Breadcrumbs",
                resolve: context =>
            {

                var store = context.GetArgumentOrValue<Store>("store");
                var cultureName = context.GetValue<string>("cultureName");

                return context.Source.Category.Outlines.GetBreadcrumbsFromOutLine(store, cultureName);

            });

            ExtendableField<ListGraphType<PropertyType>>("properties",
                arguments: new QueryArguments(new QueryArgument<ListGraphType<StringGraphType>> { Name = "names" }),
                resolve: context =>
            {
                var names = context.GetArgument<string[]>("names");
                var cultureName = context.GetValue<string>("cultureName");
                var result = context.Source.Category.Properties.ExpandByValues(cultureName);
                if (!names.IsNullOrEmpty())
                {
                    result = result.Where(x => names.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
                }
                return result;
            });

        }

        private static SeoInfo GetFallbackSeoInfo(ExpCategory source, string cultureName)
        {
            var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
            result.SemanticUrl = source.Id;
            result.LanguageCode = cultureName;
            result.Name = source.Category.Name;
            return result;
        }

        private static bool TryGetCategoryParentId(IResolveFieldContext<ExpCategory> context, out string parentId)
        {
            parentId = null;
            var outlines = context.Source.Category.Outlines;
            if (outlines.IsNullOrEmpty()) return false;

            var store = context.GetArgumentOrValue<Store>("store");

            foreach (var outline in outlines.Where(outline => outline.Items.Any(x => x.Id.Equals(store.Catalog))))
            {
                parentId = outline.Items.Take(outline.Items.Count - 1).Select(x => x.Id).LastOrDefault();

                //parentId should be a category id, not a catalog id
                if (parentId != null && parentId != store.Catalog)
                {
                    return true;
                }
            }
            return false;
        }

        private static async Task<IDictionary<string, ExpCategory>> LoadCategoriesAsync(IMediator mediator, IEnumerable<string> ids, IResolveFieldContext context)
        {
            var loadCategoryQuery = context.GetCatalogQuery<LoadCategoryQuery>();
            loadCategoryQuery.ObjectIds = ids.Where(x => x != null).ToArray();
            loadCategoryQuery.IncludeFields = context.SubFields.Values.GetAllNodesPaths();

            var response = await mediator.Send(loadCategoryQuery);
            return response.Categories.ToDictionary(x => x.Id);
        }
    }
}
