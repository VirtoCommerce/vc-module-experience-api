using System;
using System.Net.Http;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Configuration;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations
{
    public class DownstreamRequest : HttpRequestMessage
    {
        public Scenario Scenario { get; set; }
    }
}
