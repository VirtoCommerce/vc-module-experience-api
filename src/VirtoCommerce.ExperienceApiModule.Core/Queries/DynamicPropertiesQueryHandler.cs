using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class DynamicPropertiesQueryHandler :
        IQueryHandler<GetDynamicPropertyQuery, GetDynamicPropertyResponse>,
        IQueryHandler<SearchDynamicPropertiesQuery, SearchDynamicPropertiesResponse>
    {
        /// <summary>
        /// Return signle DynamicPropertyEntity
        /// </summary>
        /// <remarks>
        /// Make this method async and remove this remark
        /// </remarks>
        public virtual Task<GetDynamicPropertyResponse> Handle(GetDynamicPropertyQuery request, CancellationToken cancellationToken)
        {
            return GetDynamicPropertyResponseMock();
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

        private Task<SearchDynamicPropertiesResponse> SearchDynamicPropertiesResponseMock()
        {
            return Task.Factory.StartNew(() =>
            {
                var mockedResult = Enumerable.Range(1, 5)
                    .Select(x => new Platform.Data.Model.DynamicPropertyEntity
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

        private Task<GetDynamicPropertyResponse> GetDynamicPropertyResponseMock()
        {
            return Task.Factory.StartNew(() => new GetDynamicPropertyResponse
            {
                DynamicProperty = new Platform.Data.Model.DynamicPropertyEntity
                {
                    Id = System.Guid.NewGuid().ToString()
                }
            });
        }
    }
}
