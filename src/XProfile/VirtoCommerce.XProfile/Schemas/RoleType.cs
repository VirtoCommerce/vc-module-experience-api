using System.Linq;
using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class RoleType : ObjectGraphType<Role>
    {
        public RoleType()
        {
            Field(x => x.Description);
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.NormalizedName);
            Field("permissions", x => x.Permissions.Select(x => x.Name).ToArray()).Description("Permissions in Role");
        }
    }
}
