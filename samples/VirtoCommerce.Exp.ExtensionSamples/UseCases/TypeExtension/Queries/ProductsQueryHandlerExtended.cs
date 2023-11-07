using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class ProductsQueryHandlerExtended : SearchProductQueryHandler
    {
        public ProductsQueryHandlerExtended(ISearchProvider searchProvider, IMapper mapper, IStoreCurrencyResolver storeCurrencyResolver, IStoreService storeService, IGenericPipelineLauncher pipeline, IAggregationConverter aggregationConverter, ISearchPhraseParser phraseParser)
            : base(searchProvider, mapper, storeCurrencyResolver, storeService, pipeline, aggregationConverter, phraseParser)
        {
        }

        public override Task<SearchProductResponse> Handle(SearchProductQuery request, CancellationToken cancellationToken)
        {
            var request2 = request as SearchProductQueryExtended;

            return base.Handle(request, cancellationToken);
        }
    }
}

