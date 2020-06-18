using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class SettingType : ObjectGraphType<SettingEntry>
    {
        public SettingType()
        {
            Field(x => x.Name, nullable: true).Description("Name");
            Field(x => x.Value, nullable: true).Description("Value");
            Field(x => x.ValueType, nullable: true).Description("ValueType");
        }
    }
}
