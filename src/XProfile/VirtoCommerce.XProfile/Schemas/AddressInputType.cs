using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class AddressInputType : InputObjectGraphType
    {
        public AddressInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Name));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.AddressType));

            Field<NonNullGraphType<StringGraphType>>(nameof(Address.City));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.CountryCode));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.CountryName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Email));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.LastName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Line1));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Line2));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.MiddleName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Organization));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Phone));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.PostalCode));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.RegionId));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.RegionName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Address.Zip));
        }
    }

}
