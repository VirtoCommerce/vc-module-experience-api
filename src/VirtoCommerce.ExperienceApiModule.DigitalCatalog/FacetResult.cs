using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog
{
    public abstract class FacetResult
    {
        protected FacetResult(FacetTypes facetType)
        {
            FacetType = facetType;
        }
        public string Name { get; set; }
        public string DisplayStyle { get; set; }
        public FacetTypes FacetType { get; private set; }
    }
}
