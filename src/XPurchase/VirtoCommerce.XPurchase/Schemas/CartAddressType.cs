using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartAddressType : AddressType
    {
        public CartAddressType()
        {
            GetField(nameof(Address.FirstName)).Type = typeof(NonNullGraphType<StringGraphType>);
            GetField(nameof(Address.LastName)).Type = typeof(NonNullGraphType<StringGraphType>);
        }
    }
}
