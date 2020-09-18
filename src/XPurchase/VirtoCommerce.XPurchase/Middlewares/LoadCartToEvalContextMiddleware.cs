using System;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XPurchase.Middlewares
{
    public class LoadCartToEvalContextMiddleware : IAsyncMiddleware<PromotionEvaluationContext>, IAsyncMiddleware<PriceEvaluationContext>, IAsyncMiddleware<TaxEvaluationContext>
    {
        private readonly IMapper _mapper;
        private readonly ICartAggregateRepository _cartAggregateRepository;
        public LoadCartToEvalContextMiddleware(IMapper mapper, ICartAggregateRepository cartAggregateRepository)
        {
            _mapper = mapper;
            _cartAggregateRepository = cartAggregateRepository;
        }
        public async Task Run(PromotionEvaluationContext parameter, Func<PromotionEvaluationContext, Task> next)
        {
            var cartAggregate = await _cartAggregateRepository.GetCartAsync("default", parameter.StoreId, parameter.CustomerId, parameter.Language, parameter.Currency);
            if (cartAggregate != null)
            {
                _mapper.Map(cartAggregate, parameter);
            }

            await next(parameter);
        }

        public async Task Run(PriceEvaluationContext parameter, Func<PriceEvaluationContext, Task> next)
        {
            var cartAggregate = await _cartAggregateRepository.GetCartAsync("default", parameter.StoreId, parameter.CustomerId, parameter.Language, parameter.Currency);
            if (cartAggregate != null)
            {
                _mapper.Map(cartAggregate, parameter);
            }

            await next(parameter);
        }

        public async Task Run(TaxEvaluationContext parameter, Func<TaxEvaluationContext, Task> next)
        {
            var cartAggregate = await _cartAggregateRepository.GetCartAsync("default", parameter.StoreId, parameter.CustomerId, Language.InvariantLanguage.CultureName, parameter.Currency);
            if (cartAggregate != null)
            {
                _mapper.Map(cartAggregate, parameter);
            }

            await next(parameter);
        }
    }
}
