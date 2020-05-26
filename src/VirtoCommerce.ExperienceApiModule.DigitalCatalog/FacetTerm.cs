using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public sealed class FacetTerm : ValueObject
    {
        public string Term { get; set; }
        public long Count { get; set; }
        public bool IsSelected { get; set; }
    }
}
