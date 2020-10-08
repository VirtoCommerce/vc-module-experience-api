using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
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
            Field(x => x.Outline, nullable: true).Description(@"All parent categories ids relative to the requested catalog and concatenated with \ . E.g. (1/21/344)");
            Field(x => x.Slug, nullable: true).Description(@"Request related slug for category");
            Field(x => x.Category.Path, nullable: true).Description("Category path in to the requested catalog  (all parent categories names concatenated. E.g. (parent1/parent2))");
            Field<SeoInfoType>("seoInfo", resolve: context => context.Source.SeoInfo, description: "Request related SEO info");

            FieldAsync<CategoryType>("parent", resolve: async context =>
            {
                if (!TryGetParentId(context, out var categoryId))
                {
                    return null;
                }

                var response = await mediator.Send(new LoadCategoryQuery
                {
                    ObjectIds = new[] { categoryId },
                    IncludeFields = context.SubFields.Values.GetAllNodesPaths()
                });

                return response.Category;
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
