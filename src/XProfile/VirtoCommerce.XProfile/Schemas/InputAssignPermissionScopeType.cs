using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputAssignPermissionScopeType : InputObjectGraphType<PermissionScope>
    {
        public InputAssignPermissionScopeType()
        {
            Field(x => x.Scope);
            Field(x => x.Type);
        }
    }
}
