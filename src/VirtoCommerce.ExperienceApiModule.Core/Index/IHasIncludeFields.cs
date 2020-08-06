using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
