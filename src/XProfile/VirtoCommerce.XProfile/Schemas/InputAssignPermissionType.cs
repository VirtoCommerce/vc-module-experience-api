using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputAssignPermissionType : InputObjectGraphType<Permission>
    {
        public InputAssignPermissionType()
        {
            Field<ListGraphType<InputAssignPermissionScopeType>>(nameof(Permission.AssignedScopes));
            Field(x => x.Name);
        }
    }
}
