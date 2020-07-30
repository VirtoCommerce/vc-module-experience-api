using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputAssignRoleType : InputObjectGraphType<Role>
    {
        public InputAssignRoleType()
        {
            Field(x => x.ConcurrencyStamp, true);
            Field(x => x.Id);
            Field(x => x.Name);
            Field<NonNullGraphType<ListGraphType<InputAssignPermissionType>>>(nameof(Role.Permissions));
        }
    }
}
