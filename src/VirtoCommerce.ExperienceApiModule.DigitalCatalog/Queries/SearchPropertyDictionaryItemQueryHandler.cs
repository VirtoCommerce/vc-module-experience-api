using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;

namespace VirtoCommerce.XDigitalCatalog.Queries
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

        public async Task<SearchPropertyDictionaryItemResponse> Handle(SearchPropertyDictionaryItemQuery request, CancellationToken cancellationToken)
        {
            var result = await _propertyDictionaryItemSearchService.SearchAsync(_mapper.Map<PropertyDictionaryItemSearchCriteria>(request));

            return new SearchPropertyDictionaryItemResponse
            {
                Result = result
            };
        }
    }
}
