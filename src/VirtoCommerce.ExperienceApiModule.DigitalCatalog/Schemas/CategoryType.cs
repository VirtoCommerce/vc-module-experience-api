using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CategoryType : ObjectGraphType<ExpCategory>
    {
        public CategoryType(IMediator mediator)
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

                 var responce = await mediator.Send(loadRelatedCatalogOutlineQuery);
                 return responce.Outline;
             }, description: @"All parent categories ids relative to the requested catalog and concatenated with \ . E.g. (1/21/344)");

            FieldAsync<StringGraphType>("slug", resolve: async context =>
            {
                var outlines = context.Source.Category.Outlines;
                if (outlines.IsNullOrEmpty()) return null;

                var loadRelatedSlugPathQuery = context.GetCatalogQuery<LoadRelatedSlugPathQuery>();
                loadRelatedSlugPathQuery.Outlines = outlines;

                var responce = await mediator.Send(loadRelatedSlugPathQuery);
                return responce.Slug;
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

                return seoInfo ?? new SeoInfo
                {
                    SemanticUrl = source.Id,
                    LanguageCode = cultureName,
                    Name = source.Category.Name
                };
            }, description: "Request related SEO info");

            FieldAsync<CategoryType>("parent", resolve: async context =>
            {
                if (!TryGetParentId(context, out var categoryId))
                {
                    return null;
                }

                var loadCategoryQuery = context.GetCatalogQuery<LoadCategoryQuery>();
                loadCategoryQuery.ObjectIds = new[] { categoryId };
                loadCategoryQuery.IncludeFields = context.SubFields.Values.GetAllNodesPaths();

                var response = await mediator.Send(loadCategoryQuery);

                return response.Categories.FirstOrDefault();
            });

            Field<BooleanGraphType>("hasParent", resolve: context => TryGetParentId(context, out _));

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.Category.Outlines);

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.Category.Images);
        }

        private static bool TryGetParentId(IResolveFieldContext<ExpCategory> context, out string parentId)
        {
            parentId = context.Source.Category.ParentId;

            return parentId != null;
        }
    }
}
