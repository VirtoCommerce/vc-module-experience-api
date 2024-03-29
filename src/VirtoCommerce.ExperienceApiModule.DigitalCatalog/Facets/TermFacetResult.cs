using System;
using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class TermFacetResult : FacetResult
    {
        public TermFacetResult()
            : base(FacetTypes.Terms)
        {
        }

        public string DataType { get; set; }
        public IList<FacetTerm> Terms { get; set; } = new List<FacetTerm>();
    }
}
