using System;

namespace VirtoCommerce.ExperienceApiModule.Extension
{
    public class Price
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MinQuantity { get; set; }
        public decimal? List { get; set; }
        public decimal? Sale { get; set; }
        public decimal? Discount { get; set; }
        public string Currency { get; set; }
        public string PriceListId { get; set; }

    }
}
