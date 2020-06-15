using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services
{
    /// <summary>
    /// Represent abstraction to search in catalog api (products, categories etc)
    /// </summary>
    public interface ICatalogService
    {
        Task<Product[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup);
    }
}


