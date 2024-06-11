using GraphQL.Types;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XDigitalCatalog.Core.Schemas
{
    public class PromotionType : ObjectGraphType<Promotion>
    {
        public PromotionType()
        {
            Name = "Promotion";
            Description = "Represents promotion object";

            Field(x => x.Id, nullable: false).Description("The unique ID of the promotion.");
            Field(x => x.Name, nullable: false).Description("The name of the promotion");
            Field(x => x.Description, nullable: true).Description("Promotion description");
            Field(x => x.Type, nullable: true).Description("Promotion type");
        }
    }
}
