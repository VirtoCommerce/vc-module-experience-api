using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.XPurchase.Tests.Services
{
    public class CartProductServiceFake : CartProductService
    {
        public CartProductServiceFake(IItemService productService,
            IInventorySearchService inventoryService,
            IPricingEvaluatorService pricingEvaluatorService,
            IMapper mapper,
            IMediator mediator)
            : base(productService, inventoryService, pricingEvaluatorService, mapper, mediator)
        {
        }

        internal Task<IList<CatalogProduct>> GetProductsByIdsFakeAsync(IEnumerable<string> ids)
        {
            return base.GetProductsByIdsAsync(ids);
        }
    }
}
