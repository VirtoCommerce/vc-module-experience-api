using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsDiscountsMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMarketingPromoEvaluator _marketingEvaluator;
        private readonly IGenericPipelineLauncher _pipeline;


        public EvalProductsDiscountsMiddleware(
            IMapper mapper           
            , IMarketingPromoEvaluator marketingEvaluator
            , IGenericPipelineLauncher pipeline)
        {
            _mapper = mapper;
            _marketingEvaluator = marketingEvaluator;
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
                var promoEvalContext = new PromotionEvaluationContext
                {
                    Currency = query.CurrencyCode,
                    StoreId = query.StoreId,
                    Language = query.CultureName,
                    CustomerId = query.UserId
                };
                await _pipeline.Execute(promoEvalContext);

                //Evaluate promotions
                promoEvalContext.PromoEntries = parameter.Results.Select(x => _mapper.Map<ProductPromoEntry>(x, options =>
                {
                    options.Items["all_currencies"] = parameter.AllStoreCurrencies;
                    options.Items["currency"] = parameter.Currency;
                })).ToList();

                var promotionResults = await _marketingEvaluator.EvaluatePromotionAsync(promoEvalContext);
                var promoRewards = promotionResults.Rewards.OfType<CatalogItemAmountReward>().ToArray();
                if (promoRewards.Any())
                {
                    parameter.Results.Apply(x => x.ApplyRewards(promoRewards));
                }          
            }
            await next(parameter);
        }


    }
}
