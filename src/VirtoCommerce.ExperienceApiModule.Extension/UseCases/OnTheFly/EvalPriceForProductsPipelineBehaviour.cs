using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.OnTheFly
{
    public class EvalPriceForProductsPipelineBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly IPricingService _pricingService;
        public EvalPriceForProductsPipelineBehaviour(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (request is LoadProductRequest loadProductRequest && response is LoadProductResponse loadProductResponse)
            {
                var prices = await _pricingService.EvaluateProductPricesAsync(new PricingModule.Core.Model.PriceEvaluationContext { Currency = "USD", ProductIds = loadProductRequest.Ids });
                foreach (var product2 in loadProductResponse.Products.OfType<CatalogProduct2>())
                {
                    var price = prices.FirstOrDefault(x => x.ProductId == product2.Id);
                    if (price != null)
                    {
                        product2.Prices = new[] { new Price { List = price.List, Currency = price.Currency, PriceListId = price.PricelistId } };
                    }
                }
            }
            else if(request is SearchProductRequest searchProductRequest && response is SearchProductResponse searchProductResponse)
            {
                var prices = await _pricingService.EvaluateProductPricesAsync(new PricingModule.Core.Model.PriceEvaluationContext { Currency = "USD", ProductIds = searchProductResponse.Result.Results.Select(x => x.Id).ToArray() });
                foreach (var product2 in searchProductResponse.Result.Results.OfType<CatalogProduct2>())
                {
                    var price = prices.FirstOrDefault(x => x.ProductId == product2.Id);
                    if (price != null)
                    {
                        product2.Prices = new[] { new Price { List = price.List, Currency = price.Currency, PriceListId = price.PricelistId } };
                    }
                }
            }
        }
    }

}
