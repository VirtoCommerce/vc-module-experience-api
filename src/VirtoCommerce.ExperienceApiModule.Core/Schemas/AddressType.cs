using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class AddressType : ObjectGraphType<Address>
    {
        public AddressType()
        {
            Field<NonNullGraphType<StringGraphType>>("id", resolve: context => context.Source.Key, description: "Id");
            Field(x => x.Key, nullable: false).Description("Id");
            Field(x => x.City, nullable: false).Description("City");
            Field(x => x.CountryCode, nullable: false).Description("Country code");
            Field(x => x.CountryName, nullable: true).Description("Country name");
            Field(x => x.Email, nullable: true).Description("Email");
            Field(x => x.FirstName, nullable: true).Description("First name");
            Field(x => x.MiddleName, nullable: true).Description("Middle name");
            Field(x => x.LastName, nullable: true).Description("Last name");
            Field(x => x.Line1, nullable: false).Description("Line1");
            Field(x => x.Line2, nullable: true).Description("Line2");
            Field(x => x.Name, nullable: true).Description("Name");
            Field(x => x.Organization, nullable: true).Description("Company name");
            Field(x => x.Phone, nullable: true).Description("Phone");
            Field(x => x.PostalCode, nullable: false).Description("Postal code");
            Field(x => x.RegionId, nullable: true).Description("Region id");
            Field(x => x.RegionName, nullable: true).Description("Region name");
            Field(x => x.Zip, nullable: false).Description("Zip");
            Field(x => x.OuterId, nullable: true).Description("Outer id");
            Field<NonNullGraphType<AddressTypeType>>(nameof(Address.AddressType),
                "Address type",
                resolve: context => context.Source.AddressType);
        }
    }
}
