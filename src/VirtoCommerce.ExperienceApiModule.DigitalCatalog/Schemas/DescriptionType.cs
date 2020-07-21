using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class DescriptionType : ObjectGraphType<EditorialReview>
    {
        public DescriptionType()
        {
            Field(x => x.Id).Description("Description ID.");
            Field(x => x.ReviewType).Description("Description type.");
            Field(x => x.Content).Description("Description text.");
            Field(x => x.LanguageCode).Description("Description language code.");
        }
    }
}
