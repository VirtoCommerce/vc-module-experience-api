using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputRemoveCouponType : InputCartBaseType
    {
        public InputRemoveCouponType()
        {
            Field<StringGraphType>("couponCode");
        }
    }
}
