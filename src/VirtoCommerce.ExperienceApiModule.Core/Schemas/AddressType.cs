using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class AddressType : ObjectGraphType<AddressAggregate>
    {
        public AddressType()
        {
            Field<StringGraphType>("id", resolve: context => context.Source.Address.Key, description: "Id");
            Field(x => x.Address.Key, true).Description("Id");
            Field(x => x.Address.City, nullable: true).Description("City");
            Field(x => x.Address.CountryCode, nullable: true).Description("Country code");
            Field(x => x.Address.CountryName, nullable: true).Description("Country name");
            Field(x => x.Address.Email, nullable: true).Description("Email");
            Field(x => x.Address.FirstName, nullable: true).Description("First name");
            Field(x => x.Address.MiddleName, nullable: true).Description("Middle name");
            Field(x => x.Address.LastName, nullable: true).Description("Last name");
            Field(x => x.Address.Line1, nullable: true).Description("Line1");
            Field(x => x.Address.Line2, nullable: true).Description("Line2");
            Field(x => x.Address.Name, nullable: true).Description("Name");
            Field(x => x.Address.Organization, nullable: true).Description("Company name");
            Field(x => x.Address.Phone, nullable: true).Description("Phone");
            Field(x => x.Address.PostalCode, nullable: false).Description("Postal code");
            Field(x => x.Address.RegionId, nullable: true).Description("Region id");
            Field(x => x.Address.RegionName, nullable: true).Description("Region name");
            Field(x => x.Address.Zip, nullable: true).Description("Zip");
            Field<IntGraphType>(nameof(Address.AddressType), resolve: context => (int)context.Source.Address.AddressType);
        }
    }
}
