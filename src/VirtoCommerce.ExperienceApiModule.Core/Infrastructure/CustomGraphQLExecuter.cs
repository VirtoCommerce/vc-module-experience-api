using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.Server.Internal;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    /// <summary>
    /// Wrapper for the default GraphQL query executor with AppInsights logging
    /// </summary>
    public class CustomGraphQLExecuter<TSchema> : DefaultGraphQLExecuter<TSchema>
        where TSchema : ISchema
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomGraphQLExecuter(
            TSchema schema,
            IDocumentExecuter documentExecuter,
            IOptions<GraphQLOptions> options,
            IEnumerable<IDocumentExecutionListener> listeners,
            IEnumerable<IValidationRule> validationRules,
            TelemetryClient telemetryClient,
            IHttpContextAccessor httpContextAccessor)
            : base(schema, documentExecuter, options, listeners, validationRules)
        {
            _telemetryClient = telemetryClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ExecutionResult> ExecuteAsync(string operationName, string query, Inputs variables, IDictionary<string, object> context, IServiceProvider requestServices, CancellationToken cancellationToken = default)
        {
            // process Playground schema introspection queries without AppInsights logging
            if (operationName == "IntrospectionQuery")
            {
                return await base.ExecuteAsync(operationName, query, variables, context, requestServices, cancellationToken);
            }

            // prepare AppInsights telemerty
            var appInsightsOperationName = $"POST graphql/{operationName}";

            var requestTelemetry = new RequestTelemetry
            {
                Name = appInsightsOperationName,
                Url = new Uri(_httpContextAccessor.HttpContext.Request.GetEncodedUrl()),
            };

            requestTelemetry.Context.Operation.Name = appInsightsOperationName;
            requestTelemetry.Properties["Type"] = "GraphQL";

            var operation = _telemetryClient.StartOperation(requestTelemetry);

            // execute GraphQL query
            var result = await base.ExecuteAsync(operationName, query, variables, context, requestServices, cancellationToken);

            requestTelemetry.Success = result.Errors.IsNullOrEmpty();

            if (requestTelemetry.Success == false)
            {
                // pass an error responce code to trigger AppInsights operation failure state
                requestTelemetry.ResponseCode = "500";

                var exeptionTelemetry = new ExceptionTelemetry(result.Errors.FirstOrDefault());

                // link exception with the operation
                exeptionTelemetry.Context.Operation.ParentId = requestTelemetry.Context.Operation.Id;
                exeptionTelemetry.Context.Operation.Name = appInsightsOperationName;

                _telemetryClient.TrackException(exeptionTelemetry);
            }

            try
            {
                // sends AppInsights telemerty 
                _telemetryClient.StopOperation(operation);
            }
            catch
            {
                // do nothing if telemerty sending fails for any reason
            }

            return result;
        }
    }
}
