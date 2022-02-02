using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputChangePurchaseOrderNumber : InputCartBaseType
    {
        public InputChangePurchaseOrderNumber()
        {
            Field<StringGraphType>("purchaseOrderNumber", "Purchase Order Number");
        }
    }
}
