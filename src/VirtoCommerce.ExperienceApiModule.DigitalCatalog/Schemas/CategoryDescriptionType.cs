using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    class CategoryDescriptionType : ObjectGraphType<CategoryDescription>
    {
        public CategoryDescriptionType()
        {
            Field(x => x.Id).Description("Description ID.");
            Field(x => x.DescriptionType).Description("Description type.");
            Field(x => x.Content).Description("Description text.");
            Field(x => x.LanguageCode).Description("Description language code.");
        }
    }
}
