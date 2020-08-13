using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
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
            Field(x => x.Category.Code, nullable: false).Description("SKU of category.");
            Field(x => x.Category.Name, nullable: false).Description("Name of category.");

            Field<StringGraphType>(
                "slug",
                description: "Get slug for category. You can pass storeId and cultureName for better accuracy",
                resolve: context =>
                {
                    var storeId = context.GetValue<string>("storeId");
                    var cultureName = context.GetValue<string>("cultureName");

                    return context.Source.Category.SeoInfos.GetBestMatchingSeoInfo(storeId, cultureName)?.SemanticUrl;
                })
            .RootAlias("__object.seoInfos");

            FieldAsync<CategoryType>("parent", resolve: async context =>
            {
                if (!TryGetParentId(context, out var categoryId))
                {
                    return null;
                }

                var response = await mediator.Send(new LoadCategoryQuery
                {
                    ObjectIds = new[] { categoryId },
                    IncludeFields = context.GetAllNodesPaths()
                });

                return response.Category;
            })
            .RootAlias("__object.parentId");

            Field<BooleanGraphType>("hasParent", resolve: context => TryGetParentId(context, out _)).RootAlias("__object.parentId");

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.Category.Outlines).RootAlias("__object.outlines");

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.Category.Images);

            Field<ListGraphType<SeoInfoType>>("seoInfos", resolve: context => context.Source.Category.SeoInfos);
        }

        private static bool TryGetParentId(IResolveFieldContext<ExpCategory> context, out string parentId)
        {
            parentId = context.Source.Category.ParentId;

            return parentId != null;
        }
    }
}
