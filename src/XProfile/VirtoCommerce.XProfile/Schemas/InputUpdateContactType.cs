using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateContactType : InputMemberBaseType
    {
        public InputUpdateContactType()
        {
            Fields.FirstOrDefault(x => x.Name == nameof(Member.Id)).Type = typeof(NonNullGraphType<StringGraphType>);
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FullName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.LastName));
            Field<StringGraphType>(nameof(Contact.MiddleName));
            Field<StringGraphType>(nameof(Contact.Salutation));
            Field<StringGraphType>(nameof(Contact.PhotoUrl));
            Field<StringGraphType>(nameof(Contact.TimeZone));
            Field<StringGraphType>(nameof(Contact.DefaultLanguage));
            Field<NonNullGraphType<ListGraphType<StringGraphType>>>(nameof(Contact.Organizations));
        }
    }
}
