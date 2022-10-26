using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Tests.Services
{
    public class CartProductServiceFake : CartProductService
    {
        public CartProductServiceFake(IItemService productService,
            IInventorySearchService inventoryService,
            IPricingEvaluatorService pricingEvaluatorService,
            IMapper mapper,
            LoadUserToEvalContextService loadUserToEvalContextService)
            : base(productService, inventoryService, pricingEvaluatorService, mapper, loadUserToEvalContextService)
        {
        }

        internal Task<IList<CatalogProduct>> GetProductsByIdsFakeAsync(IEnumerable<string> ids)
        {
            return base.GetProductsByIdsAsync(ids);
        }
    }
}
