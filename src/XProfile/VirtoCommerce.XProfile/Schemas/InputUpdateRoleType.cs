using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdateRoleType : InputObjectGraphType<Role>
    {
        public InputUpdateRoleType()
        {
            Field(x => x.ConcurrencyStamp, true);
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Description, true);
            Field<NonNullGraphType<ListGraphType<InputAssignPermissionType>>>(nameof(Role.Permissions));
        }
    }
}
