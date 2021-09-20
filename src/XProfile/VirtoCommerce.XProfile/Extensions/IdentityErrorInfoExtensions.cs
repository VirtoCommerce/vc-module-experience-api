using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;
using VirtoCommerce.Platform.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class IdentityErrorInfoExtensions
    {
        public static IdentityErrorInfo MapToIdentityErrorInfo(this IdentityError x)
        {
            var error = new IdentityErrorInfo { Code = x.Code, Description = x.Description };
            if (x is CustomIdentityError customIdentityError)
            {
                error.ErrorParameter = customIdentityError.ErrorParameter;
            }

            return error;
        }
    }
}
