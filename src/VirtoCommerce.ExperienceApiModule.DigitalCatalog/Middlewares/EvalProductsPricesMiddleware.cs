using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsPricesMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPricingEvaluatorService _pricingEvaluatorService;
        private readonly IGenericPipelineLauncher _pipeline;
        private readonly ICrudService<Store> _storeService;

        public EvalProductsPricesMiddleware(
            IMapper mapper,
            IPricingEvaluatorService pricingEvaluatorService,
            IGenericPipelineLauncher pipeline,
            IStoreService storeService)
        {
            _mapper = mapper;
            _pricingEvaluatorService = pricingEvaluatorService;
            _pipeline = pipeline;
            _storeService = (ICrudService<Store>)storeService;
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

            // If prices evaluation requested
            // Always evaluate prices with PricingEvaluatorService
            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadPrices) && parameter.Results.Any())
            {
                // find Store by Id to get Catalog Id
                var store = await _storeService.GetByIdAsync(query.StoreId, StoreResponseGroup.StoreInfo.ToString());
                var evalContext = await GetPriceEvaluationContext(query, store);

                evalContext.ProductIds = parameter.Results.Select(x => x.Id).ToArray();
                var prices = await _pricingEvaluatorService.EvaluateProductPricesAsync(evalContext);

                foreach (var product in parameter.Results)
                {
                    product.AllPrices = _mapper.Map<IEnumerable<ProductPrice>>(prices.Where(x => x.ProductId == product.Id), options =>
                    {
                        options.Items["all_currencies"] = parameter.AllStoreCurrencies;
                        options.Items["currency"] = parameter.Currency;
                    }).ToList();

                    product.ApplyStaticDiscounts();
                }
            }
            await next(parameter);
        }

        protected virtual async Task<PricingModule.Core.Model.PriceEvaluationContext> GetPriceEvaluationContext(SearchProductQuery query, Store store)
        {
            var evalContext = AbstractTypeFactory<PricingModule.Core.Model.PriceEvaluationContext>.TryCreateInstance();
            evalContext.Currency = query.CurrencyCode;
            evalContext.StoreId = query.StoreId;
            evalContext.CatalogId = store?.Catalog;
            evalContext.CustomerId = query.UserId;
            evalContext.Language = query.CultureName;
            evalContext.CertainDate = DateTime.UtcNow;

            await _pipeline.Execute(evalContext);

            return evalContext;
        }
    }
}
