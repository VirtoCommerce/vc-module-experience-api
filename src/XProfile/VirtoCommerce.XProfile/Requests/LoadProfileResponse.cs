using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public class LoadProfileResponse
    {
        public IDictionary<string, Profile> Results { get; set; } = new Dictionary<string, Profile>();
    }
}
