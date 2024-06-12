using System;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Data.Middlewares
{
    public class MapPromoEvalContextMiddleware : IAsyncMiddleware<PromotionEvaluationContextCartMap>
    {
        private readonly IMapper _mapper;

        public MapPromoEvalContextMiddleware(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Run(PromotionEvaluationContextCartMap parameter, Func<PromotionEvaluationContextCartMap, Task> next)
        {
            _mapper.Map(parameter.CartAggregate, parameter.PromotionEvaluationContext);

            await next(parameter);
        }
    }
}
