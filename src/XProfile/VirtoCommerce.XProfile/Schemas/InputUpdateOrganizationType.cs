using System.Linq;
using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateOrganizationType : InputMemberBaseType
    {
        public InputUpdateOrganizationType()
        {
            Field<NonNullGraphType<StringGraphType>>("userId");
            Fields.FirstOrDefault(x => x.Name == nameof(Member.Id)).Type = typeof(NonNullGraphType<StringGraphType>);
        }
    }
}
