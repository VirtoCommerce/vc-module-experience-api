using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputCreateContactType : InputMemberBaseType
    {
        public InputCreateContactType()
        {
            Field<StringGraphType>(nameof(Contact.FullName),
                "Full name");
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FirstName),
                "First name");
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.LastName),
                "Last name");
            Field<StringGraphType>(nameof(Contact.MiddleName),
                "Middle name");
            Field<StringGraphType>(nameof(Contact.Salutation),
                "Salutation");
            Field<StringGraphType>(nameof(Contact.PhotoUrl),
                "Photo URL");
            Field<StringGraphType>(nameof(Contact.TimeZone),
                "Time zone");
            Field<StringGraphType>(nameof(Contact.DefaultLanguage),
                "Default language");
            Field<ListGraphType<StringGraphType>>(nameof(Contact.Organizations),
                "Organizations");
        }
    }
}
