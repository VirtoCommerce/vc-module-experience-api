using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, SearchPropertiesResponse>
    {
        private readonly IPropertySearchService _propertySearchService;
        private readonly IMapper _mapper;

        public SearchPropertiesQueryHandler(IPropertySearchService propertySearchService, IMapper mapper)
        {
            _mapper = mapper;
            _propertySearchService = propertySearchService;
        }

        public async Task<SearchPropertiesResponse> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
        {
            var result = await _propertySearchService.SearchPropertiesAsync(_mapper.Map<PropertySearchCriteria>(request));

            if (request.Types!=null)
            {
                result.Results = result.Results.Where(x => request.Types.Contains(x.Type)).ToList();
            }
            return new SearchPropertiesResponse
            {
                Result = result
            };
        }
    }
}
