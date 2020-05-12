using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Configuration;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Requests;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Handlers
{
    public class GetRecommendationsRequestHandler : IRequestHandler<GetRecommendationsRequest, GetRecommendationsResponse>
    {
        private readonly RecommendationOptions _options;
        private readonly IDownstreamRequestSender _sender;
        private readonly IContentRenderer _contentRenderer;
        public GetRecommendationsRequestHandler(IContentRenderer contentRenderer, IDownstreamRequestSender sender, IOptions<RecommendationOptions> options)
        {
            _sender = sender;
            _options = options.Value;
            _contentRenderer = contentRenderer;
        }

        public virtual async Task<GetRecommendationsResponse> Handle(GetRecommendationsRequest request, CancellationToken cancellationToken)
        {
            var scenario = _options.Scenarios.FirstOrDefault(x => x.Name.EqualsInvariant(request.Scenario ?? _options.DefaultScenario));
            if(scenario == null)
            {
                throw new OperationCanceledException($"scenario {request.Scenario} not found");
            }
            var content = await _contentRenderer.RenderAsync(scenario.Connection.RequestContentTemplate, request);
            var downsteamRequest = new DownstreamRequestBuilder().WithContentString(content, scenario.Connection.ContentType)
                                                                 .WithScenario(scenario)
                                                                 .WithUrl(scenario.Connection.Url)
                                                                 .WithMethod(scenario.Connection.Method ?? "GET")
                                                                 .WithHeaders(scenario.Connection.Headers)
                                                                 .Build();
            var response = await _sender.SendRequestAsync(downsteamRequest) as GetRecommendationsResponse;

            return response;
        }
    }
}
