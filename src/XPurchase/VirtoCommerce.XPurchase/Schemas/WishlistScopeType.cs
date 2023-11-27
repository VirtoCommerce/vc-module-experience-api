using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class WishlistScopeType : EnumerationGraphType
    {
        public WishlistScopeType()
        {
            AddValue(XPurchaseConstants.PrivateScope, value: XPurchaseConstants.PrivateScope, description: "Private scope");
            AddValue(XPurchaseConstants.OrganizationScope, value: XPurchaseConstants.OrganizationScope, description: "Organization scope");
        }
    }
}
