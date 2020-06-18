namespace VirtoCommerce.XPurchase.Models.Common
{
    public interface IConvertible<T>
    {
        T ConvertTo(Currency currency);
    }
}
