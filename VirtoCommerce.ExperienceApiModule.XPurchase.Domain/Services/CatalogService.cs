using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Services
{
    public class CatalogService : ICatalogService
    {
        public virtual async Task<IEnumerable<Product>> GetProductsAsync(string[] ids, Currency currency, Language language, ItemResponseGroup responseGroup = ItemResponseGroup.None)
        {
            // todo: Need implementation
            return new List<Product>()
            {
                new Product(currency, language)
                {
                    Id = ids?[0] ?? "777",
                    Name = "Hi! I Mocked!",
                },
                new Product(currency, language)
                {
                    Id = ids?[1] ?? "888",
                    Name = "Hi! I Mocked!",
                }
            };
        }
    }
}
