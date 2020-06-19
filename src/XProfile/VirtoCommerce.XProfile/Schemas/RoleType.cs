using System.Linq;
using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class RoleType : ObjectGraphType<Role>
    {
        public RoleType()
        {
            Field(x => x.Name);
            Field("permissions", x => x.Permissions.Select(x => x.Name).ToArray()).Description("Permissions in Role");
        }
    }
}
