using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class AddressInputType : InputObjectGraphType<Address>
    {
        public AddressInputType()
        {
            Field(x => x.City).Description("City");
            Field(x => x.CountryCode).Description("Country code");
            Field(x => x.CountryName).Description("Country name");
            Field(x => x.Email, true).Description("Email");
            Field(x => x.FirstName, true).Description("First name");
            Field<StringGraphType>("Id");
            Field(x => x.Key, true).Description("key");
            Field(x => x.LastName, true).Description("Last name");
            Field(x => x.Line1).Description("Line1");
            Field(x => x.Line2, true).Description("Line2");
            Field(x => x.MiddleName, true).Description("Middle name");
            Field(x => x.Name, true).Description("Name");
            Field(x => x.Organization, true).Description("Company name");
            Field(x => x.Phone, true).Description("Phone");
            Field(x => x.PostalCode).Description("Postal code");
            Field(x => x.RegionId, true).Description("Region id");
            Field(x => x.RegionName, true).Description("Region name");
            Field(x => x.Zip, true).Description("Zip");
            Field<AddressTypeEnum>(nameof(Address.AddressType));
        }
    }
}
