using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    /// <summary>
    /// AppInsights telemetry processor that skips default Graphql queries
    /// </summary>
    public class IgnorePlainGraphQLTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public IgnorePlainGraphQLTelemetryProcessor(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            // skip plain "POST /graphql" (without operation name) requests to reduce AppInsights telemetry flood
            if (item is RequestTelemetry request && request.Name?.EqualsInvariant("POST /graphql") == true)
            {
                return;
            }

            Next.Process(item);
        }
    }
}
