using System;
using System.Collections.Generic;
using System.Threading;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Server;
using GraphQL.Server.Internal;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class GraphQLExecuter<TSchema> : DefaultGraphQLExecuter<TSchema>
        where TSchema : ISchema
    {
        public GraphQLExecuter(
            TSchema schema,
            IDocumentExecuter documentExecuter,
            IOptions<GraphQLOptions> options,
            IEnumerable<IDocumentExecutionListener> listeners,
            IEnumerable<IValidationRule> validationRules)
            : base(schema, documentExecuter, options, listeners, validationRules)
        {
        }

        /// <summary>
        /// Override to set option.ThrowOnUnhandledException to true
        /// </summary>
        protected override ExecutionOptions GetOptions(string operationName, string query, Inputs variables, IDictionary<string, object> context, IServiceProvider requestServices, CancellationToken cancellationToken)
        {
            var options = base.GetOptions(operationName, query, variables, context, requestServices, cancellationToken);

            options.ThrowOnUnhandledException = true;

            return options;
        }
    }
}
