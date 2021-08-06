using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class PasswordValidationResponse
    {
        public bool Succeeded { get; set; }

        public IList<IdentityErrorInfo> Errors { get; set; }
    }
}
