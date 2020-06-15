namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public class NumericRange : ValueObject
    {
        public decimal? Lower { get; set; }
        public decimal? Upper { get; set; }
        public bool IncludeLower { get; set; }
        public bool IncludeUpper { get; set; }
    }
}
