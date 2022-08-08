using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class DescriptionType : ObjectGraphType<EditorialReview>
    {
        public DescriptionType()
        {
            Field(x => x.Id, true).Description("Description ID.");
            Field(x => x.ReviewType, true).Description("Description type.");
            Field(x => x.Content, true).Description("Description text.");
            Field(x => x.LanguageCode, true).Description("Description language code.");
        }
    }
}
