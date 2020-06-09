namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public interface IConvertible<T>
    {
        T ConvertTo(Currency currency);
    }
}
