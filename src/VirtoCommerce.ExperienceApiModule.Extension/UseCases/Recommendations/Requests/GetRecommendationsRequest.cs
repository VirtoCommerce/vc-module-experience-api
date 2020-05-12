using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Requests
{
    public class GetRecommendationsRequest : IRequest<GetRecommendationsResponse>
    {
        public string Scenario { get; set; }
        public string ItemId { get; set; }
        public string UserId { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
    }
}
