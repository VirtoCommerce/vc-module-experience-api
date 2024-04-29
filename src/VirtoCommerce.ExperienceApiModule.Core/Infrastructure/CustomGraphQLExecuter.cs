using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    /// <summary>
    /// Wrapper for the default GraphQL query executor with AppInsights logging
    /// </summary>
    public sealed class CustomGraphQLExecuter<TSchema> : DefaultGraphQLExecuter<TSchema>
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
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider)
            : base(schema, documentExecuter, options, listeners, validationRules)
        {
            _telemetryClient = serviceProvider.GetService<TelemetryClient>();
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ExecutionResult> ExecuteAsync(string operationName, string query, Inputs variables, IDictionary<string, object> context, IServiceProvider requestServices, CancellationToken cancellationToken = default)
        {
            // process Playground schema introspection queries without AppInsights logging
            if (_telemetryClient == null ||
                operationName == "IntrospectionQuery")
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
            //Replace   W3C Trace Context id generation  https://www.w3.org/TR/trace-context/ to unique value
            requestTelemetry.Context.Operation.Id = Guid.NewGuid().ToString("N");
            requestTelemetry.Context.Operation.Name = appInsightsOperationName;
            requestTelemetry.Properties["Type"] = "GraphQL";

            using var operation = _telemetryClient.StartOperation(requestTelemetry);

            // execute GraphQL query
            var result = await base.ExecuteAsync(operationName, query, variables, context, requestServices, cancellationToken);

            requestTelemetry.Success = result.Errors.IsNullOrEmpty();

            if (requestTelemetry.Success == false)
            {
                // pass an error responce code to trigger AppInsights operation failure state
                requestTelemetry.ResponseCode = "500";

                var exception = result.Errors.Count > 1
                    ? new AggregateException(result.Errors)
                    : result.Errors.FirstOrDefault() as Exception;

                var exeptionTelemetry = new ExceptionTelemetry(exception);

                // link exception with the operation
                exeptionTelemetry.Context.Operation.ParentId = requestTelemetry.Context.Operation.Id;
                exeptionTelemetry.Context.Operation.Name = appInsightsOperationName;

                _telemetryClient.TrackException(exeptionTelemetry);
            }

            return result;
        }
    }
}
