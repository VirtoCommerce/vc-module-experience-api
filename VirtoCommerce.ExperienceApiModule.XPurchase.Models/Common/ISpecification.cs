namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);
    }
}
