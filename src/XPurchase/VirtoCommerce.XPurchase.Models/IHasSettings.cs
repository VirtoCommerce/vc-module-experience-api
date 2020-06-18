namespace VirtoCommerce.XPurchase.Models
{
    public interface IHasSettings
    {
        IMutablePagedList<SettingEntry> Settings { get; }
    }
}
