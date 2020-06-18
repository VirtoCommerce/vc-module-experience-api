namespace VirtoCommerce.XPurchase.Models.Common
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);
    }
}
