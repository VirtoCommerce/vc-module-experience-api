using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    // duplicated from VirtoCommerce.XPurchase.Schemas.AddressType
    /// <summary>
    /// Platform fails to start if 2 identical class names found. Name suffix added
    /// </summary>
    public class AddressTypePro : ObjectGraphType<Address>
    {
        public AddressTypePro()
        {
            //todo add address type!
            Field(x => x.Key).Description("Id");
            Field(x => x.Name).Description("Name");
            Field(x => x.Organization, true).Description("Company name");
            Field(x => x.CountryCode).Description("Country code");
            Field(x => x.CountryName).Description("Country name");
            Field(x => x.City).Description("City");
            Field(x => x.PostalCode).Description("Postal code");
            Field(x => x.Zip, true).Description("Zip");
            Field(x => x.Line1).Description("Line1");
            Field(x => x.Line2, true).Description("Line2");
            Field(x => x.RegionId, true).Description("Region id");
            Field(x => x.RegionName, true).Description("Region name");
            Field(x => x.FirstName).Description("First name");
            Field(x => x.LastName).Description("Last name");
            Field(x => x.MiddleName, true).Description("Middle name");
            Field(x => x.Phone, true).Description("Phone");
            Field(x => x.Email, true).Description("Email");
        }
    }
}
