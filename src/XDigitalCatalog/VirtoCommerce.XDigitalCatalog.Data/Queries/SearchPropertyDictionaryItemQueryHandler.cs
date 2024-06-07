using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Core.Queries;

namespace VirtoCommerce.XDigitalCatalog.Data.Queries
{
    public class SearchPropertyDictionaryItemQueryHandler : IRequestHandler<SearchPropertyDictionaryItemQuery, SearchPropertyDictionaryItemResponse>
    {
        private readonly IPropertyDictionaryItemSearchService _propertyDictionaryItemSearchService;
        private readonly IMapper _mapper;

        public SearchPropertyDictionaryItemQueryHandler(IPropertyDictionaryItemSearchService propertyDictionaryItemSearchService, IMapper mapper)
        {
            _mapper = mapper;
            _propertyDictionaryItemSearchService = propertyDictionaryItemSearchService;
        }

        public virtual async Task<SearchPropertyDictionaryItemResponse> Handle(SearchPropertyDictionaryItemQuery request, CancellationToken cancellationToken)
        {
            var result = await _propertyDictionaryItemSearchService.SearchAsync(_mapper.Map<PropertyDictionaryItemSearchCriteria>(request), clone: false);

            return new SearchPropertyDictionaryItemResponse
            {
                Result = result
            };
        }
    }
}
