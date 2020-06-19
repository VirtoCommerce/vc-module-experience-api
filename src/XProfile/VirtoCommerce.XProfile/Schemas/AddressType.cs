using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    // duplicated from VirtoCommerce.XPurchase.Schemas.AddressType
    public class AddressType : ObjectGraphType<Address>
    {
        public AddressType()
        {
            //todo add adress type!
            Field(x => x.Key).Description("Id");
            Field(x => x.Name).Description("Name");
            Field(x => x.Organization, nullable: true).Description("Company name");
            Field(x => x.CountryCode).Description("Country code");
            Field(x => x.CountryName).Description("Country name");
            Field(x => x.City).Description("City");
            Field(x => x.PostalCode).Description("Postal code");
            Field(x => x.Zip).Description("Zip");
            Field(x => x.Line1).Description("Line1");
            Field(x => x.Line2, nullable: true).Description("Line2");
            Field(x => x.RegionId).Description("Region id");
            Field(x => x.RegionName).Description("Region name");
            Field(x => x.FirstName).Description("First name");
            Field(x => x.MiddleName, nullable: true).Description("Middle name");
            Field(x => x.LastName).Description("Last name");
            Field(x => x.Phone).Description("Phone");
            Field(x => x.Email).Description("Email");
        }
    }
}
