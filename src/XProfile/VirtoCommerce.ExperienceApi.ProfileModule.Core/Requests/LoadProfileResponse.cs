using System.Collections.Generic;
using VirtoCommerce.ExperienceApi.ProfileModule.Core.Models;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Core.Requests
{
    public class LoadProfileResponse
    {
        public IDictionary<string, Profile> Results { get; set; } = new Dictionary<string, Profile>();
    }
}
