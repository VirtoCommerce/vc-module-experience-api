using System;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XOrder.Data.Middlewares
{
    public class EvalPromoContextOrderMiddleware : IAsyncMiddleware<PromotionEvaluationContextCartMap>
    {
        private readonly IMemberOrdersService _memberOrdersService;

        public EvalPromoContextOrderMiddleware(IMemberOrdersService memberOrdersService)
        {
            _memberOrdersService = memberOrdersService;
        }

        public async Task Run(PromotionEvaluationContextCartMap parameter, Func<PromotionEvaluationContextCartMap, Task> next)
        {
            parameter.PromotionEvaluationContext.IsFirstTimeBuyer = parameter.CartAggregate.Cart.IsAnonymous
                || _memberOrdersService.IsFirstTimeBuyer(parameter.CartAggregate.Cart.CustomerId);

            await next(parameter);
        }
    }
}
