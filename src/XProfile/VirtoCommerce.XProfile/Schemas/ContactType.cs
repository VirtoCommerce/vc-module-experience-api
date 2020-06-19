using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ContactType : ObjectGraphType<Contact>
    {
        public ContactType()
        {
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Addresses);
        }
    }
}
