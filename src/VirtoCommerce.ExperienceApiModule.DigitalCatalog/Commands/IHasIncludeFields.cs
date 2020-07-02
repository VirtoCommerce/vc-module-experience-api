using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests
{
    public interface IHasIncludeFields
    {
        IEnumerable<string> IncludeFields { get; set; }
    }
}
