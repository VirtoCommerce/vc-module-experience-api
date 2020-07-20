using GraphQL.Types;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class DescriptionType : ObjectGraphType<ExpDescription>
    {
        public DescriptionType()
        {
            Field(x => x.Type).Description("Description type.");
            Field(x => x.Text).Description("Description text.");
        }
    }
}
