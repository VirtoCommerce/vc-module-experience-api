using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    //TODO: We mustn't use such general  commands that update entire contact on the xApi level. Need to use commands more close to real business scenarios instead.
    //remove in the future
    public class InputUpdateContactType : InputMemberBaseType
    {
        public InputUpdateContactType()
        {
            Fields.FirstOrDefault(x => x.Name == nameof(Member.Id)).Type = typeof(NonNullGraphType<StringGraphType>);
            Field<StringGraphType>(nameof(Contact.FullName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.LastName));
            Field<StringGraphType>(nameof(Contact.MiddleName));
            Field<StringGraphType>(nameof(Contact.Salutation));
            Field<StringGraphType>(nameof(Contact.PhotoUrl));
            Field<StringGraphType>(nameof(Contact.TimeZone));
            Field<StringGraphType>(nameof(Contact.DefaultLanguage));
            Field<ListGraphType<StringGraphType>>(nameof(Contact.Organizations));
        }
    }
}
