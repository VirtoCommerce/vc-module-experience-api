using GraphQL.Types;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas
{
    public class PromotionType : ObjectGraphType<Promotion>
    {
        public PromotionType()
        {
            Name = "Promotion";
            Description = "Represents promotion object";

            Field(d => d.Id).Description("The unique ID of the promotion.");
            Field(d => d.Name).Description("The name of the promotion");
        }
    }
}
