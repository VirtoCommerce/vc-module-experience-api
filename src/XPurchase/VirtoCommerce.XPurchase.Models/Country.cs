﻿namespace VirtoCommerce.XPurchase.Models
{
    public partial class Country
    {
        public string Name { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public CountryRegion[] Regions { get; set; }
        public string RegionType { get; set; }
    }
}
