using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class DescriptionType : ObjectGraphType<EditorialReview>
    {
        public DescriptionType()
        {
            Field(x => x.Id).Description("Description ID.");
            Field(x => x.ReviewType, nullable: true).Description("Description type.");
            Field(x => x.Content, nullable: true).Description("Description text.");
            Field(x => x.LanguageCode, nullable: true).Description("Description language code.");
        }
    }
}
