using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsTaxMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITaxProviderSearchService _taxProviderSearchService;
        private readonly IGenericPipelineLauncher _pipeline;


        public EvalProductsTaxMiddleware(IMapper mapper, ITaxProviderSearchService taxProviderSearchService, IGenericPipelineLauncher pipeline)
        {
            _mapper = mapper;
            _taxProviderSearchService = taxProviderSearchService;
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

            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            // If tax evaluation requested
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadPrices))
            {
                //Evaluate taxes
                var storeTaxProviders = await _taxProviderSearchService.SearchTaxProvidersAsync(new TaxProviderSearchCriteria { StoreIds = new[] { query.StoreId } });
                var activeTaxProvider = storeTaxProviders.Results.FirstOrDefault(x => x.IsActive);
                if (activeTaxProvider != null)
                {
                    var taxEvalContext = new TaxEvaluationContext { Currency = query.CurrencyCode, StoreId = query.StoreId, CustomerId = query.UserId };

                    await _pipeline.Execute(taxEvalContext);

                    taxEvalContext.Lines = parameter.Results.SelectMany(x => _mapper.Map<IEnumerable<TaxLine>>(x)).ToList();
                    var taxRates = activeTaxProvider.CalculateRates(taxEvalContext);
                    if (taxRates.Any())
                    {
                        parameter.Results.Apply(x => x.AllPrices.Apply(p => p.ApplyTaxRates(taxRates)));
                    }
                }
            }

            await next(parameter);
        }
    }
}
