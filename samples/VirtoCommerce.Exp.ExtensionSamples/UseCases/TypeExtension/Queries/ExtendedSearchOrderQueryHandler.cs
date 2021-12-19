using VirtoCommerce.ExperienceApiModule.XOrder;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Search.Indexed;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class ExtendedSearchOrderQueryHandler : SearchOrderQueryHandler
    {
        public ExtendedSearchOrderQueryHandler(ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            IIndexedCustomerOrderSearchService customerOrderSearchService)
            : base(searchPhraseParser, customerOrderAggregateRepository, customerOrderSearchService)
        {
        }
    }
}
