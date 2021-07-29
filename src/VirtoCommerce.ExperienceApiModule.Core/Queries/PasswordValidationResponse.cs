using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class PasswordValidationResponse
    {
        public bool Succeeded { get; set; }

        public IList<string> ErrorCodes { get; set; }
    }
}
