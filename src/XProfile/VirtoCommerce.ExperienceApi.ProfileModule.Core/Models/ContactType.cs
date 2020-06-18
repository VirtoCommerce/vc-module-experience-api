using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Core.Models
{
    public class ContactType : ObjectGraphType<Contact>
    {
        public ContactType()
        {
            Field(x => x.FirstName);
            Field(x => x.LastName);
        }
    }
}
