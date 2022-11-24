using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class GetRatingQueryHandler : IQueryHandler<GetRatingQuery, RatingEntityDto>
{
    private readonly IRatingService _ratingService;

    public GetRatingQueryHandler(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    public virtual async Task<RatingEntityDto> Handle(GetRatingQuery request, CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetForStoreAsync(request.StoreId, new[] { request.EntityId }, request.EntityType );

        return result.FirstOrDefault();
    }
}
