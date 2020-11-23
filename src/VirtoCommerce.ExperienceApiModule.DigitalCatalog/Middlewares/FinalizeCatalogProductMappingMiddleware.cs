using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class FinalizeCatalogProductMappingMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IMapper _mapper;

        public FinalizeCatalogProductMappingMiddleware(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            var store = parameter.Store;
            var language = parameter.Query.CultureName ?? store.DefaultLanguage;

            foreach (var expProduct in parameter.Results)
            {
                expProduct.AllPrices = _mapper.Map<IEnumerable<ProductPrice>>(expProduct.IndexedPrices, context =>
                {
                    context.Items["all_currencies"] = parameter.AllStoreCurrencies;
                }).ToList();

                if (parameter.Currency != null)
                {
                    expProduct.AllPrices = expProduct.AllPrices.Where(x => (x.Currency == null) || x.Currency.Equals(parameter.Currency)).ToList();
                }

                if (!expProduct.IndexedProduct.Outlines.IsNullOrEmpty())
                {
                    var outlines = expProduct.IndexedProduct.Outlines;
                    expProduct.Outline = outlines.GetOutlinePath(store.Catalog);
                    expProduct.Slug = outlines.GetSeoPath(store, language?.ToString(), null);
                }

                if (!expProduct.IndexedProduct.Reviews.IsNullOrEmpty())
                {
                    var descriptions = expProduct.IndexedProduct.Reviews;
                    expProduct.Description = descriptions.Where(x => x.ReviewType.EqualsInvariant("FullReview"))
                                                     .FirstBestMatchForLanguage(language?.ToString()) as EditorialReview;
                    if (expProduct.Description == null)
                    {
                        expProduct.Description = descriptions.FirstBestMatchForLanguage(language?.ToString()) as EditorialReview;
                    }
                }

                if (!expProduct.IndexedProduct.SeoInfos.IsNullOrEmpty())
                {
                    expProduct.SeoInfo = expProduct.IndexedProduct.SeoInfos.GetBestMatchingSeoInfo(store.Id, language?.ToString()) ?? new SeoInfo
                    {
                        SemanticUrl = expProduct.Id,
                        LanguageCode = language?.ToString(),
                        Name = expProduct.IndexedProduct.Name
                    };
                }
            }

            await next(parameter);
        }
    }
}
