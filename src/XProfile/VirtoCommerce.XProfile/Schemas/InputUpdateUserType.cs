using GraphQL.Types;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    //TODO: We mustn't use such general  commands that update entire contact on the xApi level. Need to use commands more close to real business scenarios instead.
    //remove in the future
    public class InputUpdateUserType : InputObjectGraphType
    {
        public InputUpdateUserType()
        {
            Field<NonNullGraphType<InputUpdateApplicationUserType>>("applicationUser", description: "Application user to update");
        }
    }
}
