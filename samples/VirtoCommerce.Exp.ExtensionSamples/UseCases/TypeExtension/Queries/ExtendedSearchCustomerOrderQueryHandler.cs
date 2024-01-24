using AutoMapper;
using VirtoCommerce.ExperienceApiModule.XOrder;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Search.Indexed;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class ExtendedSearchCustomerOrderQueryHandler : SearchCustomerOrderQueryHandler
    {
        public ExtendedSearchCustomerOrderQueryHandler(ISearchPhraseParser searchPhraseParser,
            ICustomerOrderAggregateRepository customerOrderAggregateRepository,
            IIndexedCustomerOrderSearchService customerOrderSearchService,
            IMapper mapper)
            : base(searchPhraseParser, customerOrderAggregateRepository, customerOrderSearchService, mapper)
        {
        }
    }
}
