using System;

namespace VirtoCommerce.XDigitalCatalog
{
    [Flags]
    public enum ExpProductResponseGroup
    {
        None = 0,
        LoadPrices = 1,
        LoadInventories = 1 << 1,
        LoadFacets = 1 << 2,
        LoadVendors = 1 << 3,
        LoadRating = 1 << 4,
        LoadWishlists = 1 << 5,
        LoadPropertyMetadata = 1 << 6,
        Full = LoadPrices | LoadInventories | LoadFacets | LoadVendors | LoadRating | LoadWishlists | LoadPropertyMetadata
    }
}
