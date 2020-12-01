using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsPricesMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly IGenericPipelineLauncher _pipeline;

        public EvalProductsPricesMiddleware(
            IMapper mapper

            , IPricingService pricingService
            , IGenericPipelineLauncher pipeline)
        {
            _mapper = mapper;
            _pricingService = pricingService;
            _pipeline = pipeline;
        }

        public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
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

            // Map indexed prices
            foreach (var expProducts in parameter.Results)
            {
                expProducts.AllPrices = _mapper.Map<IEnumerable<ProductPrice>>(expProducts.IndexedPrices, context =>
                {
                    context.Items["all_currencies"] = parameter.AllStoreCurrencies;
                }).ToList();

                if (parameter.Currency != null)
                {
                    expProducts.AllPrices = expProducts.AllPrices.Where(x => (x.Currency == null) || x.Currency.Equals(parameter.Currency)).ToList();
                }
            }

            // If prices evaluation requested
            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadPrices))
            {
                //evaluate prices only if product missed prices in the index storage
                var productsWithoutPrices = parameter.Results.Where(x => !x.IndexedPrices.Any()).ToArray();
                if (productsWithoutPrices.Any())
                {
                    var evalContext = AbstractTypeFactory<PricingModule.Core.Model.PriceEvaluationContext>.TryCreateInstance();
                    evalContext.Currency = query.CurrencyCode;
                    evalContext.StoreId = query.StoreId;
                    evalContext.CustomerId = query.UserId;
                    evalContext.Language = query.CultureName;

                    await _pipeline.Execute(evalContext);

                    evalContext.ProductIds = productsWithoutPrices.Select(x => x.Id).ToArray();
                    var prices = await _pricingService.EvaluateProductPricesAsync(evalContext);

                    foreach (var product in productsWithoutPrices)
                    {
                        product.AllPrices = _mapper.Map<IEnumerable<ProductPrice>>(prices.Where(x => x.ProductId == product.Id), options =>
                        {
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
