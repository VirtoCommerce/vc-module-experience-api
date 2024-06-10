using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Queries;
using VirtoCommerce.ExperienceApiModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Queries
{
    public class DynamicPropertyQueryHandler :
        IQueryHandler<GetDynamicPropertyQuery, GetDynamicPropertyResponse>,
        IQueryHandler<SearchDynamicPropertiesQuery, SearchDynamicPropertiesResponse>,
        IQueryHandler<SearchDynamicPropertyDictionaryItemQuery, SearchDynamicPropertyDictionaryItemResponse>
    {
        private readonly IMapper _mapper;
        private readonly ISearchPhraseParser _searchPhraseParser;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IDynamicPropertySearchService _dynamicPropertySearchService;
        private readonly IDynamicPropertyDictionaryItemsSearchService _dynamicPropertyDictionaryItemsSearchService;

        public DynamicPropertyQueryHandler(IMapper mapper, ISearchPhraseParser searchPhraseParser, IDynamicPropertyService dynamicPropertyService, IDynamicPropertySearchService dynamicPropertySearchService, IDynamicPropertyDictionaryItemsSearchService dynamicPropertyDictionaryItemsSearchService)
        {
            _mapper = mapper;
            _searchPhraseParser = searchPhraseParser;
            _dynamicPropertyService = dynamicPropertyService;
            _dynamicPropertySearchService = dynamicPropertySearchService;
            _dynamicPropertyDictionaryItemsSearchService = dynamicPropertyDictionaryItemsSearchService;
        }

        /// <summary>
        /// Return single DynamicProperty
        /// </summary>
        public virtual async Task<GetDynamicPropertyResponse> Handle(GetDynamicPropertyQuery request, CancellationToken cancellationToken)
        {
            var property = (await _dynamicPropertyService.GetDynamicPropertiesAsync(new[] { request.IdOrName })).FirstOrDefault();

            if (property == null)
            {
                var criteria = AbstractTypeFactory<DynamicPropertySearchCriteria>.TryCreateInstance();
                criteria.Keyword = request.IdOrName;
                criteria.ObjectType = request.ObjectType;
                var searchResult = await _dynamicPropertySearchService.SearchAsync(criteria);

                property = searchResult.Results.FirstOrDefault(x => x.Name == request.IdOrName);
            }

            return new GetDynamicPropertyResponse { DynamicProperty = property };
        }

        /// <summary>
        /// Return collection of DynamicPropertyEntites
        /// </summary>
        public virtual async Task<SearchDynamicPropertiesResponse> Handle(SearchDynamicPropertiesQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new DynamicPropertySearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                        .ParseFilters(request.Filter)
                                        .WithLanguage(request.CultureName)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .WithObjectType(request.ObjectType)
                                        .Build();

            var searchResult = await _dynamicPropertySearchService.SearchAsync(searchCriteria);

            return new SearchDynamicPropertiesResponse
            {
                Results = searchResult.Results,
                TotalCount = searchResult.TotalCount
            };
        }

        public virtual async Task<SearchDynamicPropertyDictionaryItemResponse> Handle(SearchDynamicPropertyDictionaryItemQuery request, CancellationToken cancellationToken)
        {
            var searchCriteria = new DynamicPropertyDictionaryItemSearchCriteriaBuilder(_searchPhraseParser, _mapper)
                                        .ParseFilters(request.Filter)
                                        .WithPropertyId(request.PropertyId)
                                        .WithLanguage(request.CultureName)
                                        .WithPaging(request.Skip, request.Take)
                                        .WithSorting(request.Sort)
                                        .Build();

            var searchResult = await _dynamicPropertyDictionaryItemsSearchService.SearchAsync(searchCriteria);

            return new SearchDynamicPropertyDictionaryItemResponse
            {
                Results = searchResult.Results,
                TotalCount = searchResult.TotalCount
            };
        }
    }
}
