using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputApplicationUserLoginType : InputObjectGraphType<ApplicationUserLogin>
    {
        public InputApplicationUserLoginType()
        {
            Field(x => x.LoginProvider);
            Field(x => x.ProviderKey);
        }
    }
}
