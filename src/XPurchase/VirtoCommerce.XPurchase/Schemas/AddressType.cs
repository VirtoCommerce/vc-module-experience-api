using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class AddressType : ObjectGraphType<Address>
    {
        public AddressType()
        {
            Field<StringGraphType>("id", resolve: context => context.Source.Key, description: "Id");
            Field(x => x.City, nullable: false).Description("City");
            Field(x => x.CountryCode, nullable: false).Description("Country code");
            Field(x => x.CountryName, nullable: false).Description("Country name");
            Field(x => x.Email, nullable: true).Description("Email");
            Field(x => x.FirstName, nullable: true).Description("First name");
            Field(x => x.Key, nullable: true).Description("Id");
            Field(x => x.LastName, nullable: true).Description("Last name");
            Field(x => x.Line1, nullable: false).Description("Line1");
            Field(x => x.Line2, nullable: true).Description("Line2");
            Field(x => x.MiddleName, nullable: true).Description("Middle name");
            Field(x => x.Name, nullable: true).Description("Name");
            Field(x => x.Organization, nullable: true).Description("Company name");
            Field(x => x.Phone, nullable: true).Description("Phone");
            Field(x => x.PostalCode, nullable: false).Description("Postal code");
            Field(x => x.RegionId, nullable: true).Description("Region id");
            Field(x => x.RegionName, nullable: true).Description("Region name");
            Field(x => x.Zip, nullable: true).Description("Zip");
            Field<IntGraphType>(nameof(Address.AddressType), resolve: context => (int)context.Source.AddressType);

        }
    }
}
