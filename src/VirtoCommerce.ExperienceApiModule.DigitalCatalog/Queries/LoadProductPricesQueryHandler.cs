using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductPricesQueryHandler : IQueryHandler<LoadProductPricesQuery, LoadProductPricesResponce>
    {
        private readonly IPricingSearchService _pricingSearchService;

        public LoadProductPricesQueryHandler(IPricingSearchService pricingSearchService)
        {
            _pricingSearchService = pricingSearchService;
        }

        public virtual async Task<LoadProductPricesResponce> Handle(LoadProductPricesQuery request, CancellationToken cancellationToken)
        {
            var priceSearchResult = await _pricingSearchService.SearchPricesAsync(new PricesSearchCriteria
            {
                ProductIds = new[] { request.ProductId }
            });

            var productPrices = priceSearchResult.Results.Select(price =>
            {
                var currency = new Currency(request.Language, price.Currency);
                var productPrice = new ProductPrice(currency)
                {
                    ProductId = price.ProductId,
                    PricelistId = price.PricelistId,
                    MinQuantity = price.MinQuantity,
                    ValidFrom = price.StartDate ?? DateTime.MinValue,
                    ValidUntil = price.EndDate ?? DateTime.MaxValue,
                    ListPrice = new Money(price.List, currency),
                    SalePrice = price.Sale != null
                        ? new Money((decimal)price.Sale, currency)
                        : new Money(price.List, currency),
                };

                return productPrice;
            });

            return new LoadProductPricesResponce
            {
                ProductPrices = productPrices
            };
        }
    }
}
