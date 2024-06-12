using System;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.XPurchase.Core.Models;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Middlewares
{
    public class LoadCartToEvalContextMiddleware : IAsyncMiddleware<PromotionEvaluationContext>, IAsyncMiddleware<PriceEvaluationContext>, IAsyncMiddleware<TaxEvaluationContext>
    {
        private readonly IMapper _mapper;
        private readonly ICartAggregateRepository _cartAggregateRepository;
        private readonly IGenericPipelineLauncher _pipeline;

        public LoadCartToEvalContextMiddleware(IMapper mapper, ICartAggregateRepository cartAggregateRepository, IGenericPipelineLauncher pipeline)
        {
            _mapper = mapper;
            _cartAggregateRepository = cartAggregateRepository;
            _pipeline = pipeline;
        }

        public async Task Run(PromotionEvaluationContext parameter, Func<PromotionEvaluationContext, Task> next)
        {
            var criteria = GetCartSearchCriteria(parameter);

            var cartAggregate = await _cartAggregateRepository.GetCartAsync(criteria, parameter.Language);
            if (cartAggregate != null)
            {
                var evalContextCartMap = new PromotionEvaluationContextCartMap
                {
                    CartAggregate = cartAggregate,
                    PromotionEvaluationContext = parameter
                };
                await _pipeline.Execute(evalContextCartMap);
            }

            await next(parameter);
        }

        public async Task Run(PriceEvaluationContext parameter, Func<PriceEvaluationContext, Task> next)
        {
            var criteria = GetCartSearchCriteria(parameter);

            var cartAggregate = await _cartAggregateRepository.GetCartAsync(criteria, parameter.Language);
            if (cartAggregate != null)
            {
                _mapper.Map(cartAggregate, parameter);
            }

            await next(parameter);
        }

        public async Task Run(TaxEvaluationContext parameter, Func<TaxEvaluationContext, Task> next)
        {
            var criteria = GetCartSearchCriteria(parameter);

            var cartAggregate = await _cartAggregateRepository.GetCartAsync(criteria, Language.InvariantLanguage.CultureName);
            if (cartAggregate != null)
            {
                _mapper.Map(cartAggregate, parameter);
            }

            await next(parameter);
        }


        protected virtual ShoppingCartSearchCriteria GetCartSearchCriteria(PromotionEvaluationContext context)
        {
            var cartSearchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();

            cartSearchCriteria.Name = "default";
            cartSearchCriteria.StoreId = context.StoreId;
            cartSearchCriteria.CustomerId = context.CustomerId;
            cartSearchCriteria.Currency = context.Currency;

            return cartSearchCriteria;
        }

        protected virtual ShoppingCartSearchCriteria GetCartSearchCriteria(PriceEvaluationContext context)
        {
            var cartSearchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();

            cartSearchCriteria.Name = "default";
            cartSearchCriteria.StoreId = context.StoreId;
            cartSearchCriteria.CustomerId = context.CustomerId;
            cartSearchCriteria.Currency = context.Currency;

            return cartSearchCriteria;
        }

        protected virtual ShoppingCartSearchCriteria GetCartSearchCriteria(TaxEvaluationContext context)
        {
            var cartSearchCriteria = AbstractTypeFactory<ShoppingCartSearchCriteria>.TryCreateInstance();

            cartSearchCriteria.Name = "default";
            cartSearchCriteria.StoreId = context.StoreId;
            cartSearchCriteria.CustomerId = context.CustomerId;
            cartSearchCriteria.Currency = context.Currency;

            return cartSearchCriteria;
        }
    }
}
