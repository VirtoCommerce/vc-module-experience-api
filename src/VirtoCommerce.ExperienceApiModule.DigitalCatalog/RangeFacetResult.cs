using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public sealed class RangeFacetResult : FacetResult
    {
        public RangeFacetResult()
            : base(FacetTypes.Range)
        {

        }
        public IList<FacetRange> Ranges { get; set; } = new List<FacetRange>();

    }
}
