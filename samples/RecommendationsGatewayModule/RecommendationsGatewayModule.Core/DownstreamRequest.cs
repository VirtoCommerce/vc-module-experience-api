using System.Net.Http;
using RecommendationsGatewayModule.Core.Configuration;

namespace RecommendationsGatewayModule.Core
{
    public class DownstreamRequest : HttpRequestMessage
    {
        public Scenario Scenario { get; set; }
    }
}
