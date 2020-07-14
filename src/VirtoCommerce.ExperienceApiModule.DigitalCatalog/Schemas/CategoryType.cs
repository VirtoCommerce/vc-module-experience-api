using System.Linq;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.Tools;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CategoryType : ObjectGraphType<Category>
    {
        public CategoryType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {
            Name = "Category";

            Field(x => x.Id, nullable: false).Description("Id of category.");
            Field(x => x.Code, nullable: false).Description("SKU of category.");
            Field(x => x.Name, nullable: false).Description("Name of category.");

            Field<StringGraphType>(
                "slug",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "storeId" },
                    new QueryArgument<StringGraphType> { Name = "cultureName" }
                ),
                resolve: context =>
                {
                    var storeId = context.GetArgument<string>("storeId"); //TODO: add extension for getting store or storeId

                    var cultureName = context.GetCultureName(nullable: true)
                                   ?? context.GetLanguage(nullable: false).CultureName;

                    return context.Source.SeoInfos
                        ?.Select(x => JObject.FromObject(x).ToObject<Tools.Models.SeoInfo>())
                        .GetBestMatchingSeoInfos(storeId, cultureName, cultureName, null)
                        .Select(x => JObject.FromObject(x).ToObject<CoreModule.Core.Seo.SeoInfo>())
                        .FirstOrDefault()
                        ?.SemanticUrl;
                });

            // TODO: maybe we need smart loading here in case we load only parentIds
            Field<CategoryType>("parent", resolve: context => context.Source.Parent);
        }
    }
}
