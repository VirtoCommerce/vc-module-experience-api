using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateRoleType : InputObjectGraphType
    {
        public InputUpdateRoleType()
        {
            Field<StringGraphType>("concurrencyStamp");
            Field<NonNullGraphType<StringGraphType>>("id");
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<StringGraphType>("description");
            Field<NonNullGraphType<ListGraphType<InputAssignPermissionType>>>(nameof(Role.Permissions));
        }
    }
}
