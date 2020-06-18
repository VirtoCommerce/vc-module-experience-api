using GraphQL.Types;
using VirtoCommerce.XPurchase.Models;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class SettingType : ObjectGraphType<SettingEntry>
    {
        public SettingType()
        {
            Field(x => x.Name, nullable: true).Description("Name");
            Field<ObjectGraphType>("value", resolve: context => context.Source.Value);
            Field(x => x.ValueType, nullable: true).Description("ValueType");
        }
    }
}
