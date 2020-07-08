using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateContactType : InputMemberBaseType
    {
        public InputUpdateContactType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.Id));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FullName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.LastName));
            Field<StringGraphType>(nameof(Contact.MiddleName));
            Field<StringGraphType>(nameof(Contact.Salutation));
            Field<StringGraphType>(nameof(Contact.PhotoUrl));
            Field<StringGraphType>(nameof(Contact.TimeZone));
            Field<StringGraphType>(nameof(Contact.DefaultLanguage));
        }
    }
}
