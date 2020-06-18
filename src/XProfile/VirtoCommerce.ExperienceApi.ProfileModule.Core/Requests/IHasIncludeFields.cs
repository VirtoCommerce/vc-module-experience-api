using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Core.Requests
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
