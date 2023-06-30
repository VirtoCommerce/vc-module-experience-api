using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public interface IHasIncludeFields
    {
        IList<string> IncludeFields { get; set; }
    }
}
