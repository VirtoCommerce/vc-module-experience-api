using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class IdentityResultResponse
    {
        public bool Succeeded { get; set; }

        public IList<IdentityErrorInfo> Errors { get; set; }
    }
}
