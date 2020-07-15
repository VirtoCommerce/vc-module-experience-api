using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Tools;
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
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "storeId" },
                    new QueryArgument<StringGraphType> { Name = "cultureName" }
                ),
                description: "Get slug for category. You can pass storeId and cultureName for better accuracy",
                resolve: context =>
                {
                    var storeId = context.GetArgument<string>("storeId"); //TODO: add extension for getting store or storeId

                    var cultureName = context.GetCultureName(nullable: true)
                                   ?? context.GetLanguage(nullable: true)?.CultureName;

                    // Simple get slug
                    if (storeId is null || cultureName is null)
                    {
                        return context.Source.Category?.SeoInfos.FirstOrDefault()?.SemanticUrl;
                    }

                    return context.Source.Category.SeoInfos
                        ?.Select(x => JObject.FromObject(x).ToObject<Tools.Models.SeoInfo>())
                        .GetBestMatchingSeoInfos(storeId, cultureName, cultureName, null)
                        .Select(x => JObject.FromObject(x).ToObject<CoreModule.Core.Seo.SeoInfo>())
                        .FirstOrDefault()
                        ?.SemanticUrl;
                });

            FieldAsync<CategoryType>("parent", resolve: async context =>
            {
                if (!TryGetParentId(context, out var categoryId))
                {
                    return null;
                }

                var responce = await mediator.Send(new LoadCategoryQuery
                {
                    Id = categoryId,
                    IncludeFields = context.SubFields.Values.GetAllNodesPaths()
                });

                return responce.Category;
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
