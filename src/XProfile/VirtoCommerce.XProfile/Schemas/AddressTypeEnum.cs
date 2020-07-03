using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class AddressTypeEnum : EnumerationGraphType
    {
        public AddressTypeEnum()
        {
            Name = "AddressTypeEnum";
            AddValue("Billing", "Billing", 1);
            AddValue("Shipping", "Shipping", 2);
            AddValue("Pickup", "Pickup", 3);
            AddValue("BillingAndShipping", "BillingAndShipping", 3);
        }
    }
}
