using System;
using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Facets
{
    [Obsolete("Use the same class from XCore.")]
    public sealed class TermFacetResult_Old : FacetResult_Old
    {
        public TermFacetResult_Old()
            : base(FacetTypes_Old.Terms)
        {
        }

        public string DataType { get; set; }
        public IList<FacetTerm_Old> Terms { get; set; } = new List<FacetTerm_Old>();
    }
}
