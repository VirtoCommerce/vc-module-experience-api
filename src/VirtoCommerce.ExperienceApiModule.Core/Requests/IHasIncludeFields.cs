using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
