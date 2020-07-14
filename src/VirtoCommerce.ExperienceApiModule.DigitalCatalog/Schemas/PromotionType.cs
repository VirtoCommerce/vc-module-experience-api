using GraphQL.Types;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class PromotionType : ObjectGraphType<Promotion>
    {
        public PromotionType()
        {
            Field(x => x.Id, nullable: false).Description("Promotion Id");
            Field(x => x.Name, nullable: false).Description("Promotion name");
            Field(x => x.Description, nullable: true).Description("Promotion description");
            Field(x => x.Type, nullable: true).Description("Promotion type");
        }
    }
}
