using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class CategoryDescriptionType : ObjectGraphType<CategoryDescription>
    {
        public CategoryDescriptionType()
        {
            Field(x => x.Id, nullable: false).Description("Description ID.");
            Field(x => x.DescriptionType, nullable: true).Description("Description type.");
            Field(x => x.Content, nullable: true).Description("Description text.");
            Field(x => x.LanguageCode, nullable: true).Description("Description language code.");
        }
    }
}
