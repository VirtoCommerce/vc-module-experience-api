using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.XDigitalCatalog.Services;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsPricesMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly IExpPricingService _pricingService;
        private readonly IGenericPipelineLauncher _pipeline;

        public EvalProductsPricesMiddleware(
            IMapper mapper
      
            , IExpPricingService pricingService
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

            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            // If prices evaluation requested
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
                    var prices = await _pricingService.EvaluateProductPricesAsync(parameter, evalContext);

                    foreach (var product in productsWithoutPrices)
                    {
                        product.AllPrices = prices.Where(x => x.ProductId == product.Id).ToList();
                    }
                }
            }

            await next(parameter);
        }


    }
}
