using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class WishlistScopeType : EnumerationGraphType
    {
        public WishlistScopeType()
        {
            AddValue(ModuleConstants.PrivateScope, value: ModuleConstants.PrivateScope, description: "Private scope");
            AddValue(ModuleConstants.OrganizationScope, value: ModuleConstants.OrganizationScope, description: "Organization scope");
        }
    }
}
