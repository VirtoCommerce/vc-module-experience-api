using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class OrderAddressType : AddressType
    {
        public OrderAddressType()
        {
            GetField(nameof(Address.FirstName)).Type = typeof(NonNullGraphType<StringGraphType>);
            GetField(nameof(Address.LastName)).Type = typeof(NonNullGraphType<StringGraphType>);
        }
    }
}
