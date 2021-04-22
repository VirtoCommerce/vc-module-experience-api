using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class DynamicPropertyQueryHandler :
        IQueryHandler<GetDynamicPropertyQuery, GetDynamicPropertyResponse>,
        IQueryHandler<SearchDynamicPropertiesQuery, SearchDynamicPropertiesResponse>,
        IQueryHandler<SearchDynamicPropertyDictionaryItemQuery, SearchDynamicPropertyDictionaryItemResponse>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IDynamicPropertySearchService _dynamicPropertySearchService;
        private readonly IDynamicPropertyDictionaryItemsSearchService _dynamicPropertyDictionaryItemsSearchService;

        public DynamicPropertyQueryHandler(IDynamicPropertyService dynamicPropertyService, IDynamicPropertySearchService dynamicPropertySearchService, IDynamicPropertyDictionaryItemsSearchService dynamicPropertyDictionaryItemsSearchService)
        {
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
                var searchResult = await _dynamicPropertySearchService.SearchDynamicPropertiesAsync(criteria);

                property = searchResult.Results.FirstOrDefault(x => x.Name == request.IdOrName);
            }

            return new GetDynamicPropertyResponse { DynamicProperty = property };
        }

        /// <summary>
        /// Return collection of DynamicPropertyEntites
        /// </summary>
        /// <remarks>
        /// Make this method async and remove this remark
        /// </remarks>
        public virtual Task<SearchDynamicPropertiesResponse> Handle(SearchDynamicPropertiesQuery request, CancellationToken cancellationToken)
        {
            return SearchDynamicPropertiesResponseMock();
        }

        public async Task<SearchDynamicPropertyDictionaryItemResponse> Handle(SearchDynamicPropertyDictionaryItemQuery request, CancellationToken cancellationToken)
        {
            var result = await _dynamicPropertyDictionaryItemsSearchService.SearchDictionaryItemsAsync(
                                    new DynamicPropertyDictionaryItemSearchCriteria
                                    {
                                        PropertyId = request.PropertyId,
                                        Keyword = request.Filter,
                                        LanguageCode = request.CultureName,
                                        Sort = request.Sort,
                                        Skip = request.Skip,
                                        Take = request.Take
                                    }
                                );

            return new SearchDynamicPropertyDictionaryItemResponse
            {
                Results = result.Results,
                TotalCount = result.TotalCount
            };
        }



        private Task<SearchDynamicPropertiesResponse> SearchDynamicPropertiesResponseMock()
        {
            return Task.Factory.StartNew(() =>
            {
                var mockedResult = Enumerable.Range(1, 5)
                    .Select(x => new DynamicProperty
                    {
                        Id = System.Guid.NewGuid().ToString()
                    })
                    .ToList();

                return new SearchDynamicPropertiesResponse
                {
                    Results = mockedResult,
                    TotalCount = mockedResult.Count
                };
            });
        }
    }
}
