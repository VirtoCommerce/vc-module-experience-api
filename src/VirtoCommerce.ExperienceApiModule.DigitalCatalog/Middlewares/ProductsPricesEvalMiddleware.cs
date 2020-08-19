using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class ProductsPricesEvalMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly IGenericPipelineLauncher _pipeline;

        public ProductsPricesEvalMiddleware(
            IMapper mapper
      
            , IPricingService pricingService
            , IGenericPipelineLauncher pipeline)
        {
            _mapper = mapper;
            _pricingService = pricingService;
            _pipeline = pipeline;
        }

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var query = parameter.Query;
            if (query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }
       
          // If promotion evaluation requested
            if (query.HasPricingFields())
            {
                //evaluate prices only if product missed prices in the index storage
                var productsWithoutPrices = parameter.Results.Where(x => !x.IndexedPrices.Any()).ToArray();
                if (productsWithoutPrices.Any())
                {
                    var priceEvalContext = new PricingModule.Core.Model.PriceEvaluationContext
                    {
                        Currency = query.CurrencyCode,
                        StoreId = query.StoreId,
                        CustomerId = query.UserId,
                        Language = query.CultureName
                    };
                    await _pipeline.Execute(priceEvalContext);
                    priceEvalContext.ProductIds = productsWithoutPrices.Select(x => x.Id).ToArray();
                    var prices = await _pricingService.EvaluateProductPricesAsync(priceEvalContext);

                    foreach (var product in productsWithoutPrices)
                    {
                        product.AllPrices = _mapper.Map<IEnumerable<ProductPrice>>(prices.Where(x => x.ProductId == product.Id), options =>
                        {
                            //TODO: Code duplication
                            options.Items["all_currencies"] = parameter.AllStoreCurrencies;
                            options.Items["currency"] = parameter.Currency;
                        }).ToList();
                    }
                }
            }
            await next(parameter);
        }


    }
}
