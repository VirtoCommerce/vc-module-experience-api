using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Requests
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
