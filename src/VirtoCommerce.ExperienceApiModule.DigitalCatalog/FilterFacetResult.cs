using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public sealed class FilterFacetResult : FacetResult
    {
        public FilterFacetResult()
            : base(FacetTypes.Filter)
        {

        }
        public int Count { get; set; }
    }
}
