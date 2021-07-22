using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchPropertiesQueryHandler : IRequestHandler<SearchPropertiesQuery, SearchPropertiesResponse>
    {
        private readonly IPropertySearchService _propertySearchService;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IMapper _mapper;

        public SearchPropertiesQueryHandler(ISearchPhraseParser searchPhraseParser, IPropertySearchService propertySearchService, IMapper mapper)
        {
            _searchPhraseParser = searchPhraseParser;
            _mapper = mapper;
            _propertySearchService = propertySearchService;
        }

        public async Task<SearchPropertiesResponse> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new PropertySearchCriteriaBuilder(_searchPhraseParser, _mapper)
                            .ParseFilters(request.Filter)
                            .WithCatalogId(request.CatalogId)
                            .WithPaging(request.Skip, request.Take)
                            .Build();

            var result = await _propertySearchService.SearchPropertiesAsync(searchCriteria);

            if (request.Types != null)
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
