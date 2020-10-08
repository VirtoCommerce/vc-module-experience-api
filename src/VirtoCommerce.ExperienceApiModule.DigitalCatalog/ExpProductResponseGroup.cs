using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.XDigitalCatalog
{
    [Flags]
    public enum ExpProductResponseGroup
    {
        None = 0,
		LoadPrices = 1,
		LoadInventories = 1 << 1,
        Full = LoadPrices | LoadInventories
    }
}
