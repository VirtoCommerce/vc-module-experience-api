using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddressType : InputObjectGraphType<Address>
    {
        public InputAddressType()
        {
            Field(x => x.City, nullable: false).Description("City");
            Field(x => x.CountryCode, nullable: false).Description("Country code");
            Field(x => x.CountryName, nullable: false).Description("Country name");
            Field(x => x.Email, nullable: false).Description("Email");
            Field(x => x.FirstName, nullable: false).Description("First name");
            Field(x => x.Key, nullable: false).Description("Id");
            Field(x => x.LastName, nullable: false).Description("Last name");
            Field(x => x.Line1, nullable: false).Description("Line1");
            Field(x => x.Line2, nullable: false).Description("Line2");
            Field(x => x.MiddleName, nullable: false).Description("Middle name");
            Field(x => x.Name, nullable: false).Description("Name");
            Field(x => x.Organization, nullable: false).Description("Company name");
            Field(x => x.Phone, nullable: false).Description("Phone");
            Field(x => x.PostalCode, nullable: false).Description("Postal code");
            Field(x => x.RegionId, nullable: false).Description("Region id");
            Field(x => x.RegionName, nullable: false).Description("Region name");
            Field(x => x.Zip, nullable: false).Description("Zip");
        }
    }
}
