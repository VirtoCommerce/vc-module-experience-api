using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class ProductsQueryHandlerExtended : SearchProductQueryHandler
    {
        public ProductsQueryHandlerExtended(ISearchProvider searchProvider, IMapper mapper, IStoreCurrencyResolver storeCurrencyResolver, ICrudService<Store> storeService, IGenericPipelineLauncher pipeline, IAggregationConverter aggregationConverter, ISearchPhraseParser phraseParser)
            : base(searchProvider, mapper, storeCurrencyResolver, storeService, pipeline, aggregationConverter, phraseParser)
        {
        }

        protected override IndexSearchRequestBuilder GetIndexedSearchRequestBuilder(SearchProductQuery request, Store store, Currency currency)
        {
            var builder = base.GetIndexedSearchRequestBuilder(request, store, currency);

            if (request is SearchProductQueryExtended)
            {
                // extract search criteria by calling builder.Build() and modify it
            }

            return builder;
        }
    }
}

