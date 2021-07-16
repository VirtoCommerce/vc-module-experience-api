using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetCountriesQueryHandler : IQueryHandler<GetCountriesQuery, GetCountriesResponse>,
        IQueryHandler<GetRegionsQuery, GetRegionsResponse>
    {
        private readonly ICountriesService _countriesService;

        public GetCountriesQueryHandler(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        public async Task<GetCountriesResponse> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _countriesService.GetCountriesAsync();
            var result = new GetCountriesResponse { Countries = countries };

            return result;
        }

        public async Task<GetRegionsResponse> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _countriesService.GetCountryRegionsAsync(request.CountryId);
            var result = new GetRegionsResponse() { Regions = regions };

            return result;
        }
    }
}
