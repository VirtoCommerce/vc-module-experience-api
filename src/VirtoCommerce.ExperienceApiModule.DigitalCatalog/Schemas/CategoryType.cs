using System.Linq;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CategoryType : ObjectGraphType<ExpCategory>
    {
        public CategoryType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Name = "Category";

            Field(x => x.Id, nullable: false).Description("Id of category.");
            Field(x => x.Category.Code, nullable: false).Description("SKU of category.");
            Field(x => x.Category.Name, nullable: false).Description("Name of category.");

            Field<StringGraphType>(
                "slug",
                description: "Get slug for category. You can pass storeId and cultureName for better accuracy",
                resolve: context => context.Source.Category?.SeoInfos?.FirstOrDefault()?.SemanticUrl);

            FieldAsync<CategoryType>("parent", resolve: async context =>
            {
                if (!TryGetParentId(context, out var categoryId))
                {
                    return null;
                }

                var response = await mediator.Send(new LoadCategoryQuery
                {
                    Id = categoryId,
                    IncludeFields = context.SubFields.Values.GetAllNodesPaths()
                });

                return response.Category;
            });

            Field<BooleanGraphType>("hasParent", resolve: context => TryGetParentId(context, out _));
        }

        private static bool TryGetParentId(IResolveFieldContext<ExpCategory> context, out string parentId)
        {
            parentId = context.Source.Category.ParentId;

            return parentId != null;
        }
    }
}
