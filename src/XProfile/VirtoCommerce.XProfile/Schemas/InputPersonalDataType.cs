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
            Field<StringGraphType>(nameof(ApplicationUser.Email),
                "Email");
            Field<StringGraphType>(nameof(Contact.FullName),
                "Full name");
            Field<StringGraphType>(nameof(Contact.FirstName),
                "First name");
            Field<StringGraphType>(nameof(Contact.LastName),
                "Last name");
            Field<StringGraphType>(nameof(Contact.MiddleName),
                "Middle name");
        }
    }
}
