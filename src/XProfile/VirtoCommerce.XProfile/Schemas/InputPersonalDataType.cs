using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputPersonalDataType : InputObjectGraphType
    {
        public InputPersonalDataType()
        {
            Field<StringGraphType>(nameof(ApplicationUser.Email));
            Field<StringGraphType>(nameof(Contact.FullName));
            Field<StringGraphType>(nameof(Contact.FirstName));
            Field<StringGraphType>(nameof(Contact.LastName));
            Field<StringGraphType>(nameof(Contact.MiddleName));
        }
    }
}
