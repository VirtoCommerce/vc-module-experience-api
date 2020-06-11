namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models
{
    public interface IHasSettings
    {
        IMutablePagedList<SettingEntry> Settings { get; }
    }
}
